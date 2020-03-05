const { Subject } = require('rxjs')

const sockets = new Map()

module.exports.socketConnect = new Subject()
module.exports.socketDisconnect = new Subject()
module.exports.socketUpdate = new Subject()

this.socketConnect.subscribe(s => sockets.set(s.socketId, s))
this.socketDisconnect.subscribe(s => sockets.delete(s.socketId))
this.socketUpdate.subscribe(s => sockets.set(s.socketId, s))

module.exports.getSocketsForGame = gameId => {
  gameId = gameId.toString()
  const socketsValues = [...sockets.values()]
  const gameSockets = socketsValues.filter(s => s.game === gameId)
  return gameSockets
}

module.exports.updateSocketsGameFromUser = async (userId, gameId) => {
  userId = userId.toString()
  gameId = gameId && gameId.toString()
  const socketsValues = [...sockets.values()]
  const userSockets = socketsValues.filter(s => s.user === userId)
  userSockets.forEach(s => this.socketUpdate.next({ ...s, game: gameId }))
}

module.exports.updateSocketsUserFromSession = async (sessionId, userId) => {
  sessionId = sessionId.toString()
  userId = userId && userId.toString()
  const socketsValues = [...sockets.values()]
  const sessionSockets = socketsValues.filter(s => s.sessionId === sessionId)
  sessionSockets.forEach(s => this.socketUpdate.next({ ...s, user: userId.toString() }))
}

module.exports.values = sockets
