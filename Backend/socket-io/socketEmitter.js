const { EventEmitter } = require('events')

const sockets = require('../rxjs/sockets')

let _io = null

class SocketEmitter extends EventEmitter {
  constructor (gameHolder) {
    super()

    const gameId = gameHolder.getJSON && gameHolder.getJSON()._id.toString()

    this._gameHolder = gameHolder
    this._gameId = gameId
    this.emit = this.emit.bind(this)
  }

  async emit (eventName, data) {
    console.log(eventName, data)
    super.emit(eventName, data)
    if (_io && sockets) {
      sockets.getSocketsForGame(this._gameId).forEach(({ socketId, user, game }) => {
        console.log('user', user.toString())
        console.log('game', game.toString())
        _io.to(socketId).emit(eventName, data)
      })
    }
  }
}

SocketEmitter.setIo = io => { _io = io }

module.exports = SocketEmitter
