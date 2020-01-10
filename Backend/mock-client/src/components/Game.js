import React, { useState } from 'react'

import { getSocket } from '../index'

export default function (props) {
  const gameToJson = () => JSON.stringify(props.game, null, 2)

  const [game, setGame] = useState(gameToJson())

  getSocket().on('playerJoined', ({ player }) => {
    const index = props.game.players.findIndex(p => p.user === player.user)
    if (index < 0) {
      props.game.players.push(player)
      setGame(gameToJson())
    }
  })

  getSocket().on('playerLeft', ({ user }) => {
    const index = props.game.players.findIndex(p => p.user === user)
    if (index > -1) {
      props.game.players.splice(index, 1)
      setGame(gameToJson())
    }
  })

  return (
    <div>
      <input type='button' value='Leave Game' onClick={props.onLeave} />
      <pre style={{ fontFamily: 'monospace' }}>{game}</pre>
    </div>
  )
}
