document.addEventListener("DOMContentLoaded", function() {
        
    var acceptButtons = document.querySelectorAll(".accept-button");

    
acceptButtons.forEach(function(button) {
    button.addEventListener("click", function () {
            
        var accountId = this.dataset.accountId;
        var gymName = "@Model.Name"; 

        var data = {
            gymName: gymName,
            accountId: accountId,
            role: "User"
        };

        fetch("/GymsController/ConfirmRequest", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Network response was not ok");
                }
                return response.json();
            })
            .then(data => {
                console.log("Request confirmed successfully:", data);
            })
            .catch(error => {
                console.error("Error confirming request:", error);
            });
    });
    });
});