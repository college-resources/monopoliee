const { EventEmitter } = require('events')

const sockets = require('../rxjs/sockets')
const gameSockets = require('../rxjs/gameSockets')

let _io = null

class SocketEmitter extends EventEmitter {
  constructor (gameHolder) {
    super()

    const gameId = gameHolder.getJSON()._id.toString()

    this._gameHolder = gameHolder
    this._gameId = gameId
    this._gameSockets = gameSockets(this._gameId)

    this.emit = this.emit.bind(this)
  }

  async emit (eventName, data) {
    console.log(eventName, data)
    super.emit(eventName, data)
    if (_io) {
      this._gameSockets.value.forEach(sid => {
        const { user, game } = sockets.values.get(sid)
        console.log('user', user.toString())
        console.log('game', game.toString())
        _io.to(sid).emit(eventName, data)
      })
    }
  }
}

SocketEmitter.setIo = io => { _io = io }

module.exports = SocketEmitter
