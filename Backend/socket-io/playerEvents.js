const SocketEmitter = require('./socketEmitter')

class PlayerEvents extends SocketEmitter {
  constructor (gameId) {
    super(gameId)

    this.onPlayerJoined = this.onPlayerJoined.bind(this)
    this.onPlayerDisconnected = this.onPlayerDisconnected.bind(this)
    this.onPlayerLeft = this.onPlayerLeft.bind(this)
  }

  onPlayerJoined (player) {
    return this.emit('playerJoined', { player })
  }

  onPlayerDisconnected (user) {
    return this.emit('playerDisconnected', { user })
  }

  onPlayerLeft (user) {
    return this.emit('playerLeft', { user })
  }
}

module.exports = PlayerEvents
