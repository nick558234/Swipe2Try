# MkDocs Setup and Testing Script

Write-Host "Setting up MkDocs for Swipe2Try Documentation..." -ForegroundColor Green

# Install Python dependencies
Write-Host "Installing MkDocs and dependencies..." -ForegroundColor Yellow
pip install -r requirements.txt

if ($LASTEXITCODE -eq 0) {
    Write-Host "Dependencies installed successfully!" -ForegroundColor Green
    
    # Test MkDocs configuration
    Write-Host "Testing MkDocs configuration..." -ForegroundColor Yellow
    mkdocs build --clean
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "MkDocs build successful!" -ForegroundColor Green
        Write-Host "You can now run 'mkdocs serve' to start the development server" -ForegroundColor Cyan
        Write-Host "Documentation will be available at http://localhost:8000" -ForegroundColor Cyan
    } else {
        Write-Host "MkDocs build failed. Please check the configuration." -ForegroundColor Red
    }
} else {
    Write-Host "Failed to install dependencies. Please check your Python installation." -ForegroundColor Red
}

Write-Host "`nAvailable commands:" -ForegroundColor Yellow
Write-Host "  mkdocs serve    - Start development server" -ForegroundColor White
Write-Host "  mkdocs build    - Build static documentation" -ForegroundColor White
Write-Host "  mkdocs gh-deploy - Deploy to GitHub Pages" -ForegroundColor White
