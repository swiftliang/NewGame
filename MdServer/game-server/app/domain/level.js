
var level = function (opts) {
    if(opts) {
        this.levels = opts.split(",");
    }else {
        this.levels = [];
    }
};

module.exports = level;

level.prototype.unlock = function(star) {
    if(star) {
        this.levels.push(star);
    }
};

level.prototype.toString = function() {
    return this.levels.toString();
};
