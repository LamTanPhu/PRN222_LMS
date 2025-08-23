// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ===== NAVBAR FUNCTIONALITY FIXES =====
document.addEventListener('DOMContentLoaded', function() {
    // Ensure navbar dropdowns work properly
    const dropdowns = document.querySelectorAll('.dropdown');
    
    dropdowns.forEach(dropdown => {
        const dropdownMenu = dropdown.querySelector('.dropdown-menu');
        const dropdownToggle = dropdown.querySelector('.dropdown-toggle');
        
        if (dropdownToggle && dropdownMenu) {
            // Show dropdown on click
            dropdownToggle.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();
                
                // Close other dropdowns
                dropdowns.forEach(otherDropdown => {
                    if (otherDropdown !== dropdown) {
                        otherDropdown.querySelector('.dropdown-menu')?.classList.remove('show');
                    }
                });
                
                // Toggle current dropdown
                dropdownMenu.classList.toggle('show');
            });
        }
    });
    
    // Close dropdowns when clicking outside
    document.addEventListener('click', function(e) {
        if (!e.target.closest('.dropdown')) {
            dropdowns.forEach(dropdown => {
                dropdown.querySelector('.dropdown-menu')?.classList.remove('show');
            });
        }
    });
    
    // Ensure navbar collapse works on mobile
    const navbarToggler = document.querySelector('.navbar-toggler');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    
    if (navbarToggler && navbarCollapse) {
        navbarToggler.addEventListener('click', function() {
            navbarCollapse.classList.toggle('show');
            
            // Ensure proper z-index and visibility
            if (navbarCollapse.classList.contains('show')) {
                navbarCollapse.style.zIndex = '1032';
                navbarCollapse.style.overflow = 'visible';
            }
        });
    }
    
    // Fix navbar positioning issues
    const navbar = document.querySelector('.navbar');
    if (navbar) {
        navbar.style.zIndex = '1030';
        navbar.style.overflow = 'visible';
    }
    
    // Ensure header has proper z-index
    const header = document.querySelector('header');
    if (header) {
        header.style.zIndex = '1030';
        header.style.overflow = 'visible';
    }
});

// ===== RESPONSIVE NAVBAR FIXES =====
window.addEventListener('resize', function() {
    const navbarCollapse = document.querySelector('.navbar-collapse');
    if (window.innerWidth >= 992) {
        // Desktop: ensure navbar is visible
        if (navbarCollapse) {
            navbarCollapse.classList.remove('show');
            navbarCollapse.style.display = 'flex';
            navbarCollapse.style.overflow = 'visible';
        }
    } else {
        // Mobile: ensure proper mobile behavior
        if (navbarCollapse) {
            navbarCollapse.style.overflow = 'visible';
        }
    }
});
