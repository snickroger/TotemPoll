var defaultData = {
  question: "",
  allowMultiple: false,
  expires: null,
  choices: []
};

var editedData = Object.assign({}, defaultData);

var app = new Vue({
  el: "#container",
  data: editedData,
  methods: {
    setExpiration: function(e) {
      if (e.target.value === "-1") {
        this.expires = null;
      } else {
        this.expires = parseInt(e.target.value);
      }
    },
    setChoices: function(e) {
      this.choices = e.target.value.split("\n").map(c => c.trim()).filter(c => c.length > 0);
    },
    getJson: function() {
      return JSON.stringify(editedData, null, 2);
    }
  }
});