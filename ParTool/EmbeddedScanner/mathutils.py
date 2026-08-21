class Vector(tuple):
    def __new__(cls, *args):
        if len(args) == 1 and hasattr(args[0], '__iter__'):
            return super().__new__(cls, args[0])
        return super().__new__(cls, args)
    
    @property
    def x(self): return self[0] if len(self) > 0 else 0.0
    @property
    def y(self): return self[1] if len(self) > 1 else 0.0
    @property
    def z(self): return self[2] if len(self) > 2 else 0.0
    @property
    def w(self): return self[3] if len(self) > 3 else 0.0
    @property
    def xyz(self): return Vector(self[:3])
    def copy(self): return Vector(self)

class Quaternion(tuple):
    def __new__(cls, *args):
        if len(args) == 1 and hasattr(args[0], '__iter__'):
            return super().__new__(cls, args[0])
        return super().__new__(cls, args)
    
    @property
    def w(self): return self[0] if len(self) > 0 else 1.0
    @property
    def x(self): return self[1] if len(self) > 1 else 0.0
    @property
    def y(self): return self[2] if len(self) > 2 else 0.0
    @property
    def z(self): return self[3] if len(self) > 3 else 0.0
    
    def __matmul__(self, other):
        if isinstance(other, (Vector, tuple, list)):
            return Vector(other)
        return self
    def copy(self): return Quaternion(self)

class Matrix(tuple):
    def __new__(cls, *args):
        if len(args) == 1 and hasattr(args[0], '__iter__'):
            return super().__new__(cls, args[0])
        return super().__new__(cls, args)
    
    def transposed(self):
        if not self:
            return self
        return Matrix(list(zip(*self)))
    def copy(self): return Matrix(self)
    def resize_4x4(self): pass
