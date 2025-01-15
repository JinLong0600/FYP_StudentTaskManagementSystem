self.addEventListener('push', function (event) {
    if (event.data) {
        const data = event.data.json();
        const options = {
            body: data.notification.body,
            icon: data.notification.icon,
            badge: data.notification.badge,
            data: data.notification.data,
            actions: data.notification.actions,
            tag: data.notification.tag,
            requireInteraction: true
        };

        event.waitUntil(
            self.registration.showNotification(data.notification.title, options)
        );
    }
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();

    if (event.action === 'markAsRead') {
        fetch('/Notification/MarkAsRead', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                taskId: event.notification.data.taskId
            })
        });
    }
    else if (event.action === 'viewTask') {
        event.waitUntil(
            clients.openWindow(event.notification.data.url)
        );
    }
    else {
        event.waitUntil(
            clients.openWindow(event.notification.data.url)
        );
    }
});