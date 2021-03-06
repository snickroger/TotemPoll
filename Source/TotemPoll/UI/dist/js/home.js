﻿var connection = $.hubConnection("/push/signalr", { useDefaultPath: false });
var hub = connection.createHubProxy("push");

hub.on("push", receivedMessage);
jQuery(loadPolls);

function receivedMessage(jsonStr) {
  var json = JSON.parse(jsonStr);
  if (json.$type === "runtime:ViewUpdated" && json.key === "poll:AllPollsView") {
    app.polls = json.content.polls;
  }
}

function loadPolls() {
  jQuery.ajax("/api/polls").done(loadPollsDone).fail(loadPollsFail);
}

function loadPollsDone(d) {
  app.polls = d.polls;
  app.loaded = true;
  app.noPolls = d.polls.length === 0;
  connection.start().done(() => hub.invoke("SubscribeView", "poll:AllPollsView"));
}

function loadPollsFail() {
  app.loaded = true;
  app.noPolls = true;
  setTimeout(loadPolls, 5000);
}

Vue.use(VueCharts);
var app = new Vue({
  el: "#container",
  data: {
    loaded: false,
    noPolls: false,
    polls: [],
    visibleGraphs: []
  },
  methods: {
    saveVote: function (e, pollId) {
      var selectedChoices = [];
      var els = jQuery(e.target).find("input:checked");
      for (var i = 0; i < els.length; i++) {
        selectedChoices.push(els.eq(i).val());
      }

      var data = { "choices": selectedChoices };
      jQuery.post(`/api/poll/${pollId}/vote`, JSON.stringify(data));
    },
    toggleGraph: function (poll) {
      var vg = this.visibleGraphs;
      for (var i = 0; i < vg.length; i++) {
        if (vg[i] === poll.id) {
          vg.splice(i, 1);
          return;
        }
      }
      vg.push(poll.id);
    },
    selectionType: function (poll) {
      return poll.selectionType === "SingleChoice" ? "radio" : "checkbox";
    }
  }
});