# SSN-Getter

A simple Proof of Concept of getting someone's SSN (personnummer) using Comviq's API to brute force the last 4 digits.

The software is only for education puposes and <b>SHOULD NOT BE ABUSED</b>.

Exception hasn't been handeled since it's just a PoC (if you get one, check your cookie since most of the issues are server-side related). 

Feel free to report issues or re-use the code to learn.

Note: You need to change the cookie in the code before using it, to do so, follow the steps below.
- Go to https://webbutik.comviq.se/student/ 
- Get all the cookies and replace the cookie content in the program and recompile it.

The program requires the persons :
- Birthday (YYYYMMDD) which can be found on [Ratsit.se](https://ratsit.se) or [Hitta.se](https://hitta.se)
- First name
- Last name

Final results example:

![Results](https://i.imgur.com/yWx4oSE.png)
