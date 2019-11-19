<p align="center">
  <img width="100" height="100" src="Assets/sockpuppet.jpg">
</p>

# SockPuppet - A SOCKS Prxy Checker

Checks the average line-seperated IP:PORT formatted proxy lists you can find on pastebin,
checks if the proxy is:
* Alive
* Able to get a HTTPS website
* Is "safe" (Returns the same HTML as without a proxy from my blog -> no injected scripts,ads,.. - no tampering)

### Usage

```
Usage: SockPuppet -input [1] -output [2] -threads [3] -timeout [4]
1: path to proxy list (format: ip:port)
2: path to output file containing all the working proxies
3: amount of proxies to try at the same time (sane values: [5])
4: how long to wait for a response before giving up in milliseconds (sane values: [5])
5: Sane thread counts: 8-16. Sane Timeout durations: 1000-10000
```

#### Example:
```SockPuppet -input pastebinlist.txt -output /home/trrbl/working-proxies -threads 128 -timeout 7500```

additionally, it will output information while its running in the following format:

```
[Down!]127.0.0.1:88
[Down!]192.168.0.1:80
[ Up! ]8.19.55.12:8080
```

The IP starts at string index 8 on every line - you could use 
`cut -c8-22` to get 
```
127.0.0.1:88
192.168.0.1:80
8.19.55.12:8080
```
on Unix.
