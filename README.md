# SSN-Getter

<b>Status: Not working</b> (seems like Comviq took action and added more security \*LOL\*). However, I'll try implementing a better solution in the future. 

A simple Proof of Concept of getting someone's SSN (personnummer) using Comviq's API to brute force the last 4 digits.

The software is only for education puposes and <b>SHOULD NOT BE ABUSED</b>.

Exeptions hasn't been handeled since it's just a PoC.

Feel free to report issues or re-use the code to learn.

Note: You may need to change the cookie in the code before using it, to do so, follow the steps below.
- Go to https://webbutik.comviq.se/student/ 
- Get the content of the cookie named "frontend"
- Replace the cookie content in the program and recompile it.

The program requires the persons:
- Birthday
- First name
- Last name

Final Results:

![Results](https://i.imgur.com/yWx4oSE.png)
