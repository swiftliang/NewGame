var skilldata = require('../../config/data/skill');

var Data = function(data) {
    var fields = {};
    data[1].forEach(function(i, k) {
        fields[i] = k;
    });

    data.splice(0, 2);

    var result = {}, item;
    data.forEach(function(k) {
        item = mapData(fiedls, k);
        result[item.id] = item;
    });

    this.data = result;
};

var mapData = function(fields, item) {
    var obj = {};
    for(var k in fields) {
        obj[k] = item[fields[k]];
    }
    return obj;
};

Data.prototype.findById = function(id) {
    return this.data[id];
};

module.exports = {
    skills: new Data(skilldata)
};