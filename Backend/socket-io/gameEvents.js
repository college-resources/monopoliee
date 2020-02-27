const SocketEmitter = require('./socketEmitter')

class GameEvents extends SocketEmitter {
  constructor (gameHolder) {
    super(gameHolder)

    this.onGameStarted = this.onGameStarted.bind(this)
    this.onGameEnded = this.onGameEnded.bind(this)
  }

  onGameIsStarting () {
    return this.emit('gameIsStarting', {})
  }

  onGameLobbyTimer (remainingSeconds) {
    return this.emit('gameLobbyTimer', { remainingSeconds })
  }

  onGameStarted (firstPlayer) {
    return this.emit('gameStarted', { firstPlayer })
  }

  onGameEnded () {
    return this.emit('gameEnded', {})
  }
}

module.exports = GameEvents
