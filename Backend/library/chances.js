const Game = require('../models/game')

module.exports = [
  {
    text: 'Πήγαινε στο Τμήμα Ανατολικών Γλωσσών. Αν περάσεις την αφετηρία πάρε 200ΔΜ.',
    action: async (userId, gameHolder) => {
      const targetPropertyLocation = 24

      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gameHolder.getPlayerEvents().onPlayerMoved(player.user, targetPropertyLocation)

      if (player.position > targetPropertyLocation) {
        gamePlayer.balance += 200

        gameHolder.getPlayerEvents().onPlayerPassedFromGo(player.user)
        gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
        gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
      }

      gamePlayer.position = targetPropertyLocation

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πήγαινε στο Τμήμα Βιοϊατρικών Επιστημών. Αν περάσεις την αφετηρία πάρε 200ΔΜ.',
    action: async (userId, gameHolder) => {
      const targetPropertyLocation = 11

      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gameHolder.getPlayerEvents().onPlayerMoved(player.user, targetPropertyLocation)

      if (player.position > targetPropertyLocation) {
        gamePlayer.balance += 200

        gameHolder.getPlayerEvents().onPlayerPassedFromGo(player.user)
        gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
        gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
      }

      gamePlayer.position = targetPropertyLocation

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Δώσε εξεταστική. Μην περάσεις από την αφετηρία, μην πάρεις 200ΔΜ.',
    action: async (userId, gameHolder) => {
      gameHolder.getPlayerEvents().onPlayerGotJailed(userId)
    }
  },
  {
    text: 'Τα σκονάκια που έδωσες στους φίλους σου ήταν λάθος. Δώσε 50ΔΜ σε κάθε παίκτη.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())
      const players = game.players
      const money = (players.length - 1) * 50
      if (gamePlayer.balance > money) {
        players.forEach(p => {
          if (p.user.toString() !== userId.toString()) {
            p.balance += 50
            gamePlayer.balance -= 50
            gameHolder.getPlayerEvents().onPlayerGotPaid(p.user, 50)
            gameHolder.getPlayerEvents().onPlayerBalanceChanged(p.user, p.balance)
            gameHolder.getPlayerEvents().onPlayerPaid(userId, 50)
            gameHolder.getPlayerEvents().onPlayerBalanceChanged(userId, gamePlayer.balance)
          }
        })
      }
      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πάρε το 52 από τη Στάση ΝΣΣ.',
    action: async (userId, gameHolder) => {
      const targetPropertyLocation = 5

      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gameHolder.getPlayerEvents().onPlayerMoved(player.user, targetPropertyLocation)

      if (player.position > targetPropertyLocation) {
        gamePlayer.balance += 200

        gameHolder.getPlayerEvents().onPlayerPassedFromGo(player.user)
        gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
        gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
      }

      gamePlayer.position = targetPropertyLocation

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πήγαινε βόλτα στο Τμήμα Νοσηλευτικής.',
    action: async (userId, gameHolder) => {
      const targetPropertyLocation = 39

      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gameHolder.getPlayerEvents().onPlayerMoved(player.user, targetPropertyLocation)

      if (player.position > targetPropertyLocation) {
        gamePlayer.balance += 200

        gameHolder.getPlayerEvents().onPlayerPassedFromGo(player.user)
        gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
        gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
      }

      gamePlayer.position = targetPropertyLocation

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πήγαινε στην αφετηρία (Πάρε 200ΔΜ).',
    action: async (userId, gameHolder) => {
      const targetLocation = 0

      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gameHolder.getPlayerEvents().onPlayerMoved(player.user, targetLocation)

      gamePlayer.balance += 200

      gameHolder.getPlayerEvents().onPlayerPassedFromGo(player.user)
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      gamePlayer.position = targetLocation

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πέρασες την θεωρία του μαθήματος. Παρε 50ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 50
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 50)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πέρασες την θεωρία του μαθήματος. Παρε 150ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 150
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 150)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πέρασες την θεωρία του μαθήματος. Παρε 100ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 100
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 100)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  }
]
