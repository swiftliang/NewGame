var Code = require('../../../../consts/code');
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
Handler.prototype.entry = function(msg, session, next) {
	var token = msg.token, self = this;
	if(!token) {
		next(new Error('invalid entry request: empty token'), {code: Code.FAILED});
		return;
	}
	var MDGame;
	async.waterfall([
		function(cb) {
			self.app.rpc.auth.authRemote.auth(session, token, cb);
		},function(code, user, cb) {
			if(code !== Code.SUCCESS) {
				next(null, {code: code});
				return;
			}

			if(!user) {
				next(null, {code: Code.ENTRY.FA_USER_NOT_EXIST});
				return;
			}

			mdDao.getGameInfoByuId(user.uid, cb);
		}, function(err, res, cb) {
			if(!err) {
				mdDao.CreateGameInfo(user.uid, function(err, player) {
					if(err) {
						next(null, {code: Code.FA_PLAYER_CREATE_FAILED});
						return;
					}

					res = player;
				});
			}
			MDGame = res;
			self.app.get('sessionService').kick(uid, cb);
		}, function(cb) {
			session.bind(uid, cb);
		}, function(cb) {
			if(!MDGame || MDGame.length === 0) {
				next(null, {code: Code.SUCCESS});
				return;
			}

			MDGame = MDGame[0];
			session.set('playername', MDGame.name);
			session.on('closed', onUserLeave.bind(null, self.app));
			session.pushAll(cb);
			app.PlayerManager.add(MDGame.uid, MDGame);
		}
	], function(err) {
		if(err) {
			next(err, {code: Code.FAILED});
			return;
		}
	});

	next(null, {code: Code.SUCCESS, gameInfo: MDGame});
};

var onUserLeave = function(app, session, reason) {
	if(!session || !session.uid) {
		return;
	}

	app.PlayerManager.remove(session.uid);
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
		payload: JSON.stringify({code: 200, msg: 'publish message is ok.'})
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
		payload: JSON.stringify({code: 200, msg: 'subscribe message is ok.'})
	};
  next(null, result);
};

Handler.prototype.StartGame = function(msg, session, next) {
	var result = {
		code: Code.SUCCESS
	};
	next(null, result);
};

Handler.prototype.AddStar = function(msg, session, next) {
	var result = {

	};
	next(null, result);
};

Handler.prototype.AddIcon = function(msg, session, next) {
	next(null, result);
};