
var skill = function (opts) {
    if(opts) {
        this.skills = opts.split(",");
    }
};

module.exports = skill;

skill.prototype.contains = function(id) {
    return this.skills.indexOf(id) != -1;
};

skill.prototype.learn = function(id) {
    this.skills.push(id);
};

skill.prototype.toString = function() {
    return this.skills.toString();
};