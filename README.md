# LiveLisp
Recent found backup of my university project where I'm trying to implement something lispy
around 2006-2008

Can compute fibonacci

```lisp
>>>> (defun fibonacci (n &optional (a 0) (b 1) (acc ()))
  (if (zerop n)
      (nreverse acc)
      (fibonacci (1- n) b (+ a b) (cons a acc))))
FIBONACCI
>>>> (fibonacci 5)
(0 1 1 2 3)
>>>> (fibonacci 16)
(0 1 1 2 3 5 8 13 21 34 55 89 144 233 377 610)
```

## License
MIT
