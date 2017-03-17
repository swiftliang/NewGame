
var level = function (opts) {
    if(opts) {
        this.levels = opts.split(",");
    }
};

module.exports = level;

level.prototype.unlock = function(star) {
    if(star) {
        this.levels.push(star);
    }
};

level.prototype.toString = function() {
    this.levels.toString();
};
