var skill = require('./skill');
var level = require('./level');
var Persistent = require('./persistent');
var util = require('util');

var MDGame = function(opts) {
    this.uid = opts.uid;
    this.coin = opts.coin;
    this.coinUpdataTime = opts.coinUpdataTime;
    this.stars = opts.stars;
    this.starUpdataTime = opts.starUpdataTime;
    this.levels = level(opts.level);
    this.skills = skill(opts.skills);
};

util.inherits(MDGame, Persistent);

module.exports = MDGame;