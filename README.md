# Simple Proxy Checker

Checks the average line-seperated IP:PORT formatted proxy lists you can find on pastebin,
checks if the proxy is:
* Alive
* Able to get a HTTPS website
* Is "safe" (Returns the same HTML as without a proxy from my blog -> no injected scripts,ads,.. - no tampering)

### Usage

`proxy-checker -i ProxyList.txt -o WorkingProxies.txt -t AmountOfThreads`

additionally, it will output information while its running in the following format:

```
[Down!]127.0.0.1:88
[Down!]192.168.0.1:80
[ Up! ]8.19.55.12:8080
```

The IP starts at string index 8 on every line - you could use 
`cut -c8-22` to get the `IP:PORT` part on Unix.
