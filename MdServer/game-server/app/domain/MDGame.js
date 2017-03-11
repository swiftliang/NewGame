var skill = require('./skill');
var level = require('./level');

var MDGame = function(opts) {
    this.uid = opts.uid;
    this.coin = opts.coin;
    this.stars = opts.stars;
    this.levels = level(opts.level);
    this.skills = skill(opts.skills);
};

module.exports = MDGame;