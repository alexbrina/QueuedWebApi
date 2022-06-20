$concurrent_clients = 48
$execution_time = "24s"

# run tests
Write-Output "starting load test..."
k6 run --vus $concurrent_clients --duration $execution_time .\k6-work-endpoint.js
