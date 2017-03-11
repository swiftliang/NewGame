var skill = require('./skill');

var MDGame = function(opts) {
    this.uid = opts.uid;
    this.cion = opts.cion;
    this.stars = opts.stars;
    this.skills = skill(opts.skills);
};

module.exports = MDGame;