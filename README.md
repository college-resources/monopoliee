# Monopoliee
### International Hellenic University (IHU) Themed Monopoly

#### PC version page [monopoliee.cores.gr](https://monopoliee.cores.gr)

#### Android version available in the GitHub [releases](https://github.com/college-resources/monopoliee/releases) page.

#### Restrictions
- 2 - 4 players
- WebGL browser support needed for PC version

# <p align="center">![](https://raw.githubusercontent.com/college-resources/monopoliee/refs/heads/master/Unity/Assets/Resources/board.png)</p>

# Setup

## Dependencies

1. [Unity 2019.3.7](https://unity3d.com/get-unity/download/archive)

2. [MongoDB Community Server](https://www.mongodb.com/download-center/community) (You can do a custom installation and choose **only** the Server module)

3. [NodeJS 10.X](https://nodejs.org/dist/latest-v10.x) (Recommended version: [NodeJS 10.18.0 x64](https://nodejs.org/dist/latest-v10.x/node-v10.18.0-x64.msi))

4. [Git]([https://git-scm.com/download/win](https://git-scm.com/downloads))

## Downloading the project

Navigate to the folder where you want the project to be saved, open a terminal and run the following command
```bash
git clone https://gitlab.com/college-resources/monopoliee.git
```
You may be asked to login to your GitHub account. This is because the project is owned by a private group.

## Preparing the API (Backend)

1. Navigate to `monopoliee/Backend` and run
```bash
npm install
```

If you get any `node-pre-gyp ERR!` errors during `npm install`, make sure you have all [bcrypt dependencies](https://github.com/kelektiv/node.bcrypt.js#dependencies) installed and run `npm install` again. **Windows users can also download our [prebuilt bcrypt package](https://cdn.discordapp.com/attachments/650336477674340352/650481763457695754/bcrypt.zip) and extract it inside `Backend/node_modules`.** After installing it, run `npm install` to make sure everything is fine.

## Preparing the UI (Unity)

1. Open Unity Editor and open the `monopoliee/Unity` folder, as a Unity Project.

2. Go to `File -> Build Settings` and select all scenes in `Scenes to build`

3. Select `WebGl` in `Platform` and click the `Switch Platform` button, if you don't see a Unity icon next to `WebGL`.

4. If `Development Build` is checked, uncheck it.

5. Click `Build`. (Depending on your setup, building may take ages to complete)

# API Documentation

## Authentication API

| Endpoint | Method | Parameters | Returns | Description |
|----------|--------|------------|---------|-------------|
| `/auth/register` | POST | username: String<br/>password: String | Redirect to `/auth/session` | Registers new user |
| `/auth/login` | POST | username: String<br/>password: String | Redirect to `/auth/session` | Logs user in |
| `/auth/session` | GET | none | User Object | Returns current logged in user |
| `/auth/logout` | GET | none | Success message | Logs current logged in user out |

## Game API

| Endpoint | Method | Parameters | Returns | Description |
|----------|--------|------------|---------|-------------|
| `/game/new` | POST | seats: Int | Game Object | Creates a new game with seats number equal to `seats` and joins it |
| `/game/join` | POST | game_id: MongoId | Game Object | Joins the game that has _id equal to `game_id` |
| `/game/list` | GET | none | Array of Game Objects | Returns the list of games with status `waitingForPlayers` or `running` |
| `/game/current` | GET | none | Game Object | Returns the game that the logged in user is currently playing |
| `/game/prices` | GET | none | Prices Object | Returns the prices object from config.json|

## Player API

| Endpoint | Method | Parameters | Returns | Description |
|----------|--------|------------|---------|-------------|
| `/player/roll-dice` | GET | none | Dice object | Rolls the dice, moves the player and returns the roll |
| `/player/end-turn` | GET | none | Player Object | Completes the turn of the current user and returns the next player |

## Transaction API
| Endpoint | Method | Parameters | Returns | Description |
|----------|--------|------------|---------|-------------|
| `/transaction/buy-current-property` | GET | none | Property Object | Buys the property that the player is standing on |

# Socket.io Events

## Game Events
- gameIsStarting
- gameLobbyTimer
- gameStarted
- gameEnded

## Player Events
- playerJoined
- playerDisconnected
- playerLeft
- playerRolledDice
- playerMoved
- playerTurnChanged
- playerPlaysAgain
- playerPassedFromGo
- playerGotPaid
- playerPaid
- playerBalanceChanged
- playerSteppedOnChance
- playerSteppedOnCommunityChest
- playerSteppedOnTax
- playerGotJailed
- playerGotFurloughed

## Property Events
- propertyOwnerChanged
- propertyMortgagedChanged
