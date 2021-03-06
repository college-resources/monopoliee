const http = require('http')

require('dotenv').config()

const bodyParser = require('body-parser')
const connectMongo = require('connect-mongo')
const cors = require('cors')
const express = require('express')
const expressSession = require('express-session')
const logger = require('morgan')
const mongoose = require('mongoose')
const proxy = require('http-proxy-middleware')
const sharedSession = require('express-socket.io-session')
const socketIo = require('socket.io')

const gameMiddleware = require('./middleware/game')

const authRouter = require('./routes/auth')
const gameRouter = require('./routes/game')
const playerRouter = require('./routes/player')
const transactionRouter = require('./routes/transaction')

const GameError = require('./library/gameError')
const GameManager = require('./library/gameManager')

const SocketEmitter = require('./socket-io/socketEmitter')

const sockets = require('./rxjs/sockets')

const app = express()
const server = http.Server(app)
const io = socketIo(server)

const cookieSecret = process.env.COOKIE_SECRET || 'key'
const mongoUri = process.env.MONGODB_CLUSTER
  ? `mongodb+srv://${process.env.MONGODB_USER}:${process.env.MONGODB_PASS}@${
    process.env.MONGODB_CLUSTER
  }`
  : 'mongodb://localhost/monopoliee'

if (!process.env.NODE_ENV || process.env.NODE_ENV !== 'production') {
  mongoose.set('debug', function (coll, method, query, doc) {
    console.log('\x1B[33m' + new Date().toISOString() + '\x1B[0m', 'Query executed:', coll, method, query, doc)
  })
}

mongoose.set('useCreateIndex', true)
mongoose.set('useUnifiedTopology', true)
mongoose.set('useFindAndModify', false)
const mongooseConnecting = mongoose.connect(mongoUri, { useNewUrlParser: true })

const MongoStore = connectMongo(expressSession)

const session = expressSession({
  secret: cookieSecret,
  resave: false,
  saveUninitialized: true,
  cookie: { secure: false },
  store: new MongoStore({ mongooseConnection: mongoose.connection })
})

io.use(sharedSession(session))
SocketEmitter.setIo(io)

io.on('connection', async socket => {
  const getSessionUser = async () => {
    socket.handshake.session.reload(err => {
      if (err) console.error(err)
    })

    return socket.handshake.session.user
  }

  await getSessionUser().then(async user => {
    const socketToAdd = {
      sessionId: socket.handshake.sessionID,
      socketId: socket.id
    }

    if (user) {
      socketToAdd.user = user && user._id.toString()
      const gameManager = new GameManager(user)
      await gameManager.init().then(() => {
        const currentGame = gameManager.current()
        socketToAdd.game = currentGame && currentGame._id.toString()
      })
    }
    sockets.socketConnect.next(socketToAdd)
  })

  socket.on('disconnect', () => getSessionUser().then(async user => {
    if (user) {
      const lastGameManager = new GameManager(user)
      await lastGameManager.init().then(async () => {
        const lastGameHolder = lastGameManager.getGameHolder()
        if (lastGameHolder) {
          const playerEvents = lastGameHolder.getPlayerEvents()
          await playerEvents.onPlayerDisconnected(user._id)
        }
      })
    }
    const s = sockets.values.get(socket.id)
    sockets.socketDisconnect.next(s)
  }))
})

app.use((req, _, next) => {
  console.time('session_load ' + req.path)
  next()
})

app.use(cors())
app.use(logger('dev'))
app.use(bodyParser.json())
app.use(bodyParser.urlencoded({ extended: false }))
app.use(session)

app.use((req, _, next) => {
  console.timeEnd('session_load ' + req.path)
  next()
})

app.use((req, _, next) => {
  console.time('game_load ' + req.path)
  next()
})

app.use(gameMiddleware)

app.use((req, _, next) => {
  console.timeEnd('game_load ' + req.path)
  next()
})

app.use('/auth', authRouter)
app.use('/game', gameRouter)
app.use('/player', playerRouter)
app.use('/transaction', transactionRouter)

if (!process.env.NODE_ENV || process.env.NODE_ENV !== 'production') {
  app.use(proxy('/mock-client', {
    target: 'http://localhost:3001',
    changeOrigin: true,
    ws: true,
    pathRewrite: {
      '^/mock-client': '/'
    }
  }))
  app.use(express.static('../Unity/Build'))
} else {
  app.use(express.static('./unity-build'))
}

app.use(function (err, req, res, next) {
  if (err instanceof GameError) {
    return res.status(400).json({ error: err.toJSON() })
  } else {
    console.error(err.stack)
    res.status(500).send({ error: { message: 'Internal Server Error' } })
  }
})

mongooseConnecting
  .then(() => {
    const port = process.env.PORT || 3000
    server.listen(port, err => {
      if (err) throw err
      console.log(`Listening on port ${port}...`)
    })
  })
  .catch(err => {
    console.error(err)
  })
