var CODE = require('../../../../../shared/code');
var async = require('async');
var mdDao = require('../../../dao/mdDao');

module.exports = function(app) {
  return new Handler(app);
};

var Handler = function(app) {
  this.app = app;
};

/**
 * New client entry.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.enter = function(msg, session, next) {
	var token = msg.token, self = this;
	if(!token) {
		next(new Error('invalid entry request: empty token'), {code: CODE.FAILED});
		return;
	}
	var gameInfo;
	async.waterfall([
		function(cb) {
			self.app.rpc.auth.authRemote.auth(session, token, cb);
		},function(code, user, cb) {
			if(code !== CODE.OK) {
				next(null, {code: code});
				return;
			}

			if(!user) {
				next(null, {code: CODE.ENTRY.FA_USER_NOT_EXIST});
				return;
			}

			mdDao.getGameInfoByuId(user.uid, cb);
		}, function(err, res, cb) {

			if(!res) {
				next(null, {code: CODE.ENTRY.FA_PLAYER_CREATE_FAILED});
				return;
			}

			gameInfo = res;
			self.app.get('sessionService').kick(gameInfo.uid, cb);
		}, function(cb) {
			session.bind(gameInfo.uid, cb);
		}, function(cb) {
			if(!gameInfo || gameInfo.length === 0) {
				next(null, {code: CODE.FAILED});
				return;
			}

			self.app.get('PlayerManager').add(gameInfo.uid, gameInfo);
			session.set('playerid', gameInfo.uid);
			session.on('closed', onUserLeave.bind(null, self.app));
			session.pushAll(cb);
		}
	], function(err) {
		if(err) {
			next(err, {code: CODE.FAILED});
			return;
		}


		next(null, {code: CODE.OK, gameInfo: {coin: gameInfo.coin, levels: gameInfo.levels.toString(), skills: gameInfo.skills.toString()}});//JSON.stringify(gameInfo)
	});
};

var onUserLeave = function(app, session, reason) {
	if(!session || !session.uid) {
		return;
	}

	app.get('PlayerManager').remove(session.uid);
};
/**
 * Publish route for mqtt connector.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.publish = function(msg, session, next) {
	var result = {
		topic: 'publish',
		payload: JSON.stringify({code: CODE.OK, msg: 'publish message is ok.'})
	};
  next(null, result);
};

/**
 * Subscribe route for mqtt connector.
 *
 * @param  {Object}   msg     request message
 * @param  {Object}   session current session object
 * @param  {Function} next    next step callback
 * @return {Void}
 */
Handler.prototype.subscribe = function(msg, session, next) {
	var result = {
		topic: 'subscribe',
		payload: JSON.stringify({code: CODE.OK, msg: 'subscribe message is ok.'})
	};
  next(null, result);
};

Handler.prototype.StartGame = function(msg, session, next) {
	var result = {
		code: CODE.OK
	};
	next(null, result);
};

Handler.prototype.UpdateStar = function(msg, session, next) {
	var player = self.app.PlayerManager.get(session.uid);
	var result;
	if(!player) {
		player.stars += msg.star;
		player.levels.UpdateStar(msg.level, msg.star);
		result = {
			code: CODE.OK,
			star: player.stars,
			level: player.levels.toString()
		};
		player.Level();
	} else {
		result = {
			code: CODE.FAILED
		};
	}
	next(null, result);
};

Handler.prototype.UpdateCoin = function(msg, session, next) {
	var player = self.app.PlayerManager.get(session.uid);
	var result;
	if(!player) {
		player.coin = msg.coin;
		result = {
			code: CODE.OK,
			coin: player.coin
		};
		player.Coin();
	} else {
		result = {
			code: CODE.FAILED
		};
	}
	next(null, result);
};