// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
"use strict";
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var user = $('#Username').val();

    //$(document).getElementById("sendButton").disabled = true;

    connection.on("PrivateReciever", (user, message) => {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = user + ": " + msg;

        $('#messagesList').append('<li class="list-group-item text-primary">' + encodedMsg + '</li>');
        $('#messagesList').scrollTop($('#messagesList').prop('scrollHeight'));
    });
    connection.on("ReceiveMessage", (user, message, timestamp) => {
        //console.log(message);
        //var msg = message;
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = user + ": " + msg + "<br/>" + timestamp;

        $('#messagesList').append('<li class="list-group-item">' + encodedMsg + '</li>');
        $('#messagesList').scrollTop($('#messagesList').prop('scrollHeight'));
    });

    connection.on("notifyDirect", () => {
        alert("You have a new notification");
    });

    connection.start().then(() => {
            connection.invoke("AddUser", user)
    });

    /*connection.start().then(() => {
        $(document).getElementById("sendButton").disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });*/

    var $messagecontent = $('#messageInput');
    $messagecontent.keyup(function (e) {
        //var user = document.getElementById("userInput").value;
        if (e.keyCode == 13) {

            var message = $messagecontent.val().trim();
            if (message.length == 0)
                return false;

            connection.invoke("TestSend", user, message).catch((err) => {
                return console.error(err.toString());
            });
            $messagecontent.val('');
        }
    });
    $messagecontent.keydown(function (e) {
        if (e.keyCode == 191) {
            var message = $messagecontent.val().trim()

            if (message.length ==  0)
                return false;

            connection.invoke("ChangeReciever", user, message).catch((err) => {
                return console.error(err.toString());
            });
            $messagecontent.val('');
        }
    });

    /*$(document).getElementById("sendButton").addEventListener("click", (event) => {
        var user = $(document).getElementById("userInput").val().trim();
        var message = $(document).getElementById("messageInput").val().trim();
        connection.invoke("SendMessage", user, message).catch((err) => {
            return console.error(err.toString());
        });
        event.preventDefault();
    });*/

});

