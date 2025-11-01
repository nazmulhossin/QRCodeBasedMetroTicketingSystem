document.addEventListener('DOMContentLoaded', function () {
    // Password toggle visibility
    document.getElementById('passwordToggle')?.addEventListener('click', function () {
        const passwordInput = document.getElementById('password');
        const icon = this.querySelector('i');
        const PASSWORD_TYPE = "password";
        const TEXT_TYPE = "text";

        if (passwordInput.type === PASSWORD_TYPE) {
            passwordInput.type = TEXT_TYPE;
            icon.classList.remove("fa-eye");
            icon.classList.add("fa-eye-slash");
        } else {
            passwordInput.type = PASSWORD_TYPE;
            icon.classList.remove('fa-eye-slash');
            icon.classList.add('fa-eye');
        }
    });
});