import turtle as t
from random import randint
from colorsys import hsv_to_rgb
from time import time


def main() -> None:
    start_time = time()
    hue = 0
    bound_x = t.window_width() / 2
    bound_y = t.window_height() / 2
    t.speed("fastest")
    t.bgcolor("black")
    while time() - start_time < 300:
        t.pencolor(hsv_to_rgb(hue, 1, 1))
        t.forward(randint(10, 50))
        t.right(randint(-90, 90))
        hue += 0.005
        if hue >= 1:
            hue = 0
        x, y = t.pos()
        t.penup()
        if x > bound_x:
            x = -bound_x
        elif x < -bound_x:
            x = bound_x
        if y > bound_y:
            y = -bound_y
        elif y < -bound_y:
            y = bound_y
        t.goto(x, y)
        t.pendown()
    t.done()


if __name__ == "__main__":
    main()
