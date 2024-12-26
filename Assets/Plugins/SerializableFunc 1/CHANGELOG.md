### 1.1.0

Added support for generic arguments. Works like a generic Func, where the last generic argument is the return type of the function. Having only the return value will look for getter properties aswell. Having > 1 generic argument will only look for suitable public methods