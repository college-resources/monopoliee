const SocketEmitter = require('./socketEmitter')

class GameEvents extends SocketEmitter {
  constructor (gameId) {
    super(gameId)

    this.onGameStarted = this.onGameStarted.bind(this)
    this.onGameEnded = this.onGameEnded.bind(this)
  }

  onGameStarted () {
    return this.emit('gameStarted', {})
  }

  onGameEnded () {
    return this.emit('gameEnded', {})
  }
}

module.exports = GameEvents
