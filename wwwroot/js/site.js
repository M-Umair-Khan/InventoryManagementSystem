// Site-wide JavaScript functions

// Initialize tooltips
document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// Confirm delete function
function confirmDelete() {
    return confirm('Are you sure you want to delete this item? This action cannot be undone.');
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// Auto-generate product code
function generateProductCode() {
    const prefix = 'PRD';
    const random = Math.floor(Math.random() * 10000).toString().padStart(4, '0');
    return prefix + random;
}

// Calculate line total
function calculateLineTotal(quantity, unitPrice, discountPercent = 0) {
    const total = quantity * unitPrice;
    const discount = total * (discountPercent / 100);
    return total - discount;
}

// Validate stock availability
function validateStockAvailability(productId, requestedQuantity) {
    // This would typically make an AJAX call to check stock
    return true;
}

// Export table to Excel
function exportToExcel(tableId, filename = 'export') {
    const table = document.getElementById(tableId);
    const ws = XLSX.utils.table_to_sheet(table);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Sheet1");
    XLSX.writeFile(wb, filename + '.xlsx');
}

// Print table
function printTable(tableId) {
    const table = document.getElementById(tableId);
    const win = window.open('', '', 'height=700,width=700');
    win.document.write('<html><head>');
    win.document.write('<title>Print</title>');
    win.document.write('<style>');
    win.document.write('table { border-collapse: collapse; width: 100%; }');
    win.document.write('th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }');
    win.document.write('th { background-color: #f2f2f2; }');
    win.document.write('@media print { .no-print { display: none; } }');
    win.document.write('</style>');
    win.document.write('</head><body>');
    win.document.write(table.outerHTML);
    win.document.write('</body></html>');
    win.document.close();
    win.print();
}

// Refresh dashboard data
function refreshDashboard() {
    location.reload();
}

// Auto-refresh every 5 minutes (300000 ms)
setTimeout(refreshDashboard, 300000);

// Handle form submission with loading indicator
document.addEventListener('submit', function (e) {
    const form = e.target;

    // Check if it's a form that needs validation
    if (form.classList.contains('needs-validation')) {
        if (!form.checkValidity()) {
            e.preventDefault();
            e.stopPropagation();
        } else {
            // Only show loading indicator if form is valid
            const submitBtn = form.querySelector('button[type="submit"]');
            if (submitBtn && !submitBtn.disabled) {
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
                submitBtn.disabled = true;
            }
        }
        form.classList.add('was-validated');
    } else {
        // For forms without validation, still show loading but don't validate
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn && !submitBtn.disabled) {
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
            submitBtn.disabled = true;
        }
    }
});