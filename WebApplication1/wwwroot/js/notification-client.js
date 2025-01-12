async function registerForPushNotifications() {
    try {
        // Register service worker first
        const registration = await navigator.serviceWorker.register('/js/service-worker.js');
        console.log('Service Worker registered');

        // Get push subscription with proper VAPID key
        const vapidPublicKey = 'BClsYka9-CPPr95g2rUI1C67n_Pgqy3N_98Wg3uBbohLeFCnArI8t69n4gUP4JKn2adAEekda3ZUhAcuTjVOlaA'; // Make sure this is your actual VAPID key
        const convertedVapidKey = urlBase64ToUint8Array(vapidPublicKey);
        
        const subscription = await registration.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: convertedVapidKey
        });

        // Send subscription to backend
        try {
            const response = await fetch('/api/utilities/subscribe', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    endpoint: subscription.endpoint,
                    p256dh: btoa(String.fromCharCode.apply(null, new Uint8Array(subscription.getKey('p256dh')))),
                    auth: btoa(String.fromCharCode.apply(null, new Uint8Array(subscription.getKey('auth'))))
                })
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Subscription failed:', response.status, errorText);
            }
        } catch (error) {
            console.error('Subscription error:', error);
        }

        console.log('Push notification subscription successful');
    } catch (error) {
        console.error('Failed to subscribe to push notifications:', error);
    }
}

async function unregisterForPushNotifications() {
    try {
        // Register service worker first
        try {
            const response = await fetch('/api/utilities/unsubscribe', {
                method: 'POST',
            });

            if (!response.ok) {
                const errorText = await response.text();
                console.error('Unsubscription failed:', response.status, errorText);
            }
        } catch (error) {
            console.error('Unsubscription error:', error);
        }

        console.log('Push notification Unsubscription successful');
    } catch (error) {
        console.error('Failed to unsubscription to push notifications:', error);
    }
}

// Improved base64 to Uint8Array conversion
function urlBase64ToUint8Array(base64String) {
    try {
        const padding = '='.repeat((4 - base64String.length % 4) % 4);
        const base64 = (base64String + padding)
            .replace(/-/g, '+')
            .replace(/_/g, '/');

        const rawData = window.atob(base64);
        const outputArray = new Uint8Array(rawData.length);

        for (let i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    } catch (error) {
        console.error('Error converting VAPID key:', error);
        throw error;
    }
}

