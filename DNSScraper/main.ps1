$fqdn = "*.digicert.com"

$hostEntry = [System.Net.Dns]::GetHostByName($fqdn)

ForEach ($a in $hostEntry.AddressList)
{
	$a.IPAddressToString
}
