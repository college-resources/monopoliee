const Game = require('../models/game')

const GameEvents = require('../socket-io/gameEvents')
const PlayerEvents = require('../socket-io/playerEvents')

class GameHolder {
  constructor (game) {
    if (!game) {
      throw new Error('game is required')
    }

    this._game = game
    this._gameEvents = new GameEvents(game.id)
    this._playerEvents = new PlayerEvents(game.id)
    // TODO: Subscribe to change stream

    this.getJSON = this.getJSON.bind(this)
    this.getPlayerEvents = this.getPlayerEvents.bind(this)
    this.update = this.update.bind(this)
  }

  getJSON () {
    return this._game.toJSON()
  }

  getGameEvents () {
    return this._gameEvents
  }

  getPlayerEvents () {
    return this._playerEvents
  }

  async update () {
    this._game = await Game.findById(this._game.id)
    return this
  }
}

GameHolder.getGameHolder = async gameId => {
  const game = await Game.findById(gameId)
  if (game) {
    return new GameHolder(game)
  }
}

module.exports = GameHolder