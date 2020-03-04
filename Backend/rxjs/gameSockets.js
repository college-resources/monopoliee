const { BehaviorSubject } = require('rxjs')
const { filter, skip } = require('rxjs/operators')

const sockets = require('./sockets')

const gameSocketsInstances = new Map()

module.exports = gameId => {
  gameId = gameId.toString()

  if (gameSocketsInstances.has(gameId)) {
    return gameSocketsInstances.get(gameId)
  }

  let socketsForGame = sockets.getSocketsForGame(gameId).map(s => s.socketId)
  const gameSockets = new BehaviorSubject(socketsForGame)
  socketsForGame = null

  const addGameSocket = s => !gameSockets.value.includes(s.socketId) && gameSockets.next([...gameSockets.value, s.socketId])
  const deleteGameSocket = s => gameSockets.value.includes(s.socketId) && gameSockets.next(gameSockets.value.filter(sid => sid !== s.socketId))

  const subscriptions = [
    sockets.socketConnect
      .pipe(filter(s => s.game === gameId))
      .subscribe(addGameSocket),

    sockets.socketDisconnect
      .pipe(filter(s => s.game === gameId))
      .subscribe(deleteGameSocket),

    sockets.socketUpdate
      .pipe(filter(s => s.game === gameId))
      .subscribe(addGameSocket),

    sockets.socketUpdate
      .pipe(filter(s => s.game !== gameId))
      .subscribe(deleteGameSocket),

    gameSockets
      .pipe(skip(1), filter(({ length }) => !length))
      .subscribe(() => {
        subscriptions.forEach(s => s.unsubscribe())
        gameSocketsInstances.delete(gameId)
      })
  ]

  gameSocketsInstances.set(gameId, gameSockets)
  return gameSockets
}
