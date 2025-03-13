document.addEventListener('DOMContentLoaded', async () => {
    const stripe = Stripe('pk_test_51OvLPTRseCm2355thbbCegRv8QQsI19Ntc3NXdjPU2kWPFHTzO6J7hg13WUnNW9Zr9I5OmO0RHGLzLKNQMIIczYK00MX4ZfLbs');
    
    const sessionId = document.getElementById('sessionId').getAttribute('data-session-id');
    console.log(sessionId);

    stripe.redirectToCheckout({ sessionId });
});