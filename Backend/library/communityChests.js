const Game = require('../models/game')

module.exports = [
  {
    text: 'Πήγαινε στην Αφετηρία.',
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
    text: 'Έκανες την απαλλακτική εργασία σου. Πάρε 100ΔΜ.',
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
  },
  {
    text: 'Από την ανταλλαγή βιβλίων στον Εύδοξο παίρνεις 50ΔΜ.',
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
    text: 'Απάντησες σε απορία στην ομαδική της σχολής. Πάρε 10ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 10
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 10)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Στρώθηκες και διάβασες. Πάρε 25ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 25
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 25)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Πέρασες μάθημα. Πάρε 20ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 20
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 20)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Διάβασες για την εξεταστική στις διακοπές των Χριστουγέννων. Πάρε 100ΔΜ.',
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
  },
  {
    text: 'Η γραμματεία ενημέρωσε την Πυθία με τα μαθήματα της τελευταίας εξεταστικής. Πάρε 200ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 200
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Κράτησες θέση για τους φίλους σου στις εξετάσεις, πάρε 50ΔΜ από κάθε παίκτη.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())
      const players = game.players
      const money = (players.length - 1) * 50
      if (gamePlayer.balance > money) {
        players.forEach(p => {
          if (p.user.toString() !== userId.toString()) {
            p.balance -= 50
            gamePlayer.balance += 50
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
    text: 'Βοήθησες τους φίλους σου στην εξεταστική. Πάρε 10ΔΜ από κάθε παίκτη.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())
      const players = game.players
      const money = (players.length - 1) * 10
      if (gamePlayer.balance > money) {
        players.forEach(p => {
          if (p.user.toString() !== userId.toString()) {
            p.balance -= 10
            gamePlayer.balance += 10
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
    text: 'Έμεινες στο εργαστηριακό μέρος μαθήματος. Χάνεις 50ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance -= 50
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 50)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

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
    text: 'Δεν πρόλαβες να πάρεις καφέ πριν τις εξετάσεις και ήσουν λάσπη. Χάνεις 150ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance -= 150
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 150)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: 'Δεν πήρες βιβλία από τον Εύδοξο. Χάνεις 100ΔΜ.',
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance -= 100
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 100)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  }
]
