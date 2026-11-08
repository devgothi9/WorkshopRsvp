param(
    [string]$BaseUrl = 'http://localhost:5080',
    [switch]$SkipFormPosts
)

$ErrorActionPreference = 'Stop'
$BaseUrl = $BaseUrl.TrimEnd('/')
$results = [System.Collections.Generic.List[object]]::new()

function Assert-Check([string]$Name, [bool]$Passed) {
    if (-not $Passed) { throw "FAIL: $Name" }
    $results.Add([pscustomobject]@{ Check = $Name; Result = 'PASS' })
}

foreach ($route in @('/', '/Home/Privacy', '/Workshops', '/Workshops/RsvpForm', '/Workshops/Registrations', '/css/site.css', '/lib/bootstrap/dist/css/bootstrap.min.css', '/lib/jquery-validation/dist/jquery.validate.min.js')) {
    $response = Invoke-WebRequest "$BaseUrl$route" -UseBasicParsing
    Assert-Check "GET $route returns 200" ($response.StatusCode -eq 200)
}

$registrations = (Invoke-WebRequest "$BaseUrl/Workshops/Registrations" -UseBasicParsing).Content
foreach ($name in @('Alice Martin', 'Bilal Khan', 'Chloe Tremblay')) {
    Assert-Check "Registrations displays $name" ($registrations.Contains($name))
}
Assert-Check 'Registrations contains three data rows' (([regex]::Matches($registrations, '<tr>')).Count -eq 4)
Assert-Check 'Registrations displays accommodation Yes and No' ($registrations.Contains('>Yes</span>') -and $registrations.Contains('>No</span>'))
$index = (Invoke-WebRequest "$BaseUrl/Workshops" -UseBasicParsing).Content
Assert-Check 'Landing page displays database count' ($index -match 'badge bg-secondary ms-1">3</span>')

if (-not $SkipFormPosts) {
    $form = Invoke-WebRequest "$BaseUrl/Workshops/RsvpForm" -UseBasicParsing -SessionVariable browserSession
    $token = [regex]::Match($form.Content, 'name="__RequestVerificationToken"[^>]*value="([^"]+)"').Groups[1].Value
    Assert-Check 'Form includes antiforgery token' (-not [string]::IsNullOrWhiteSpace($token))
    $invalid = Invoke-WebRequest "$BaseUrl/Workshops/Confirm" -UseBasicParsing -Method Post -WebSession $browserSession -Body @{
        __RequestVerificationToken = $token; FullName = ''; Email = 'invalid'; WorkshopTitle = ''
    }
    Assert-Check 'Invalid form redisplays required and email validation' ($invalid.Content.Contains('Please enter your full name.') -and $invalid.Content.Contains('Please enter a valid email address.') -and $invalid.Content.Contains('Please choose a workshop.'))
    $valid = Invoke-WebRequest "$BaseUrl/Workshops/Confirm" -UseBasicParsing -Method Post -WebSession $browserSession -Body @{
        __RequestVerificationToken = $token; FullName = 'Lab Four Test'; Email = 'lab4-test@example.com'; WorkshopTitle = 'EF Core Intro'; NeedsAccommodation = 'true'
    }
    Assert-Check 'Valid form displays confirmation and accommodation' ($valid.Content.Contains('Thanks for registering, Lab Four Test!') -and $valid.Content.Contains('lab4-test@example.com') -and $valid.Content.Contains('Our team will contact you'))
    $after = (Invoke-WebRequest "$BaseUrl/Workshops/Registrations" -UseBasicParsing).Content
    Assert-Check 'Form submission does not change seeded registrations' (-not $after.Contains('Lab Four Test') -and ([regex]::Matches($after, '<tr>')).Count -eq 4)
    $status = 0
    try {
        Invoke-WebRequest "$BaseUrl/Workshops/Confirm" -UseBasicParsing -Method Post -Body @{ FullName = 'No Token' } | Out-Null
    } catch {
        if ($_.Exception.Response) { $status = [int]$_.Exception.Response.StatusCode } else { throw }
    }
    Assert-Check 'POST without antiforgery token returns 400' ($status -eq 400)
}

$results | Format-Table -AutoSize
Write-Output "All $($results.Count) checks passed against $BaseUrl."
