self.addEventListener('push', function(event) {
    const options = {
        body: event.data.text(),
        icon: '/path/to/icon.png',
        badge: '/path/to/badge.png',
        data: {
            url: self.location.origin // or any specific URL you want to open on click
        }
    };

    event.waitUntil(
        self.registration.showNotification('Task Notification', options)
    );
});

self.addEventListener('notificationclick', function(event) {
    event.notification.close();
    
    if (event.notification.data && event.notification.data.url) {
        event.waitUntil(
            clients.openWindow(event.notification.data.url)
        );
    }
});