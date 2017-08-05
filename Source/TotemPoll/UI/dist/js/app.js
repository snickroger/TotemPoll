var connection = $.hubConnection("/push/signalr", { useDefaultPath: false });
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
  connection.start().done(() => hub.invoke("SubscribeView", "poll:AllPollsView"));
}

function loadPollsFail() {
  app.loaded = true;
}

var app = new Vue({
  el: "#container",
  data: {
    loaded: false,
    polls: []
  }
});