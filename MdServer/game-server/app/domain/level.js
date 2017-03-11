
var level = function (opts) {
    this.levels = opts.split(",");
};

module.exports = level;

level.prototype.unlock = function(star) {
    this.levels.push(star);
};

level.prototype.toString = function() {
    this.levels.toString();
}