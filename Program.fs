module Program

open System
open System.Drawing


type MyTurtle() =
    let canvas: Bitmap = new Bitmap(800, 600)
    let graphics: Graphics = Graphics.FromImage canvas

    let mutable position: PointF = PointF(0.0f, 0.0f)
    let mutable heading: float32 = 0.0f
    let mutable color: Color = Color.Black

    let centerX: float32 = canvas.Width / 2 |> float32
    let centerY: float32 = canvas.Height / 2 |> float32

    do
        if OperatingSystem.IsWindowsVersionAtLeast(6, 1) then
            graphics.Clear Color.Black
            graphics.TranslateTransform(centerX, centerY)
            graphics.ScaleTransform(1.0f, -1.0f)

    override this.Finalize() : unit =
        ignore this // Required to silence the "unused variable" warning from VS Code

        if OperatingSystem.IsWindowsVersionAtLeast(6, 1) then
            canvas.Dispose()
            graphics.Dispose()

    member this.forward(distance: float32) : unit =
        ignore this // Required to silence the "unused variable" warning from VS Code

        let radian: float32 = heading * MathF.PI / 180.0f

        let newPosition: PointF =
            PointF(position.X + distance * cos radian, position.Y + distance * sin radian)

        if OperatingSystem.IsWindowsVersionAtLeast(6, 1) then
            use pen: Pen = new Pen(color, 2.0f)
            graphics.DrawLine(pen, position, newPosition)

            position <- newPosition
            let bound: PointF = PointF(centerX, centerY)

            // Handle screen boundaries
            if position.X > bound.X then
                position.X <- -bound.X
            elif position.X < -bound.X then
                position.X <- bound.X

            if position.Y > bound.Y then
                position.Y <- -bound.Y
            elif position.Y < -bound.Y then
                position.Y <- bound.Y

    member this.Position: PointF =
        ignore this // Required to silence the "unused variable" warning from VS Code
        position

    member this.left(angle: float32) : unit =
        ignore this // Required to silence the "unused variable" warning from VS Code
        heading <- heading - angle

    member this.right(angle: float32) : unit =
        ignore this // Required to silence the "unused variable" warning from VS Code
        heading <- heading + angle

    member this.goTo(x: float32, y: float32) : unit =
        ignore this // Required to silence the "unused variable" warning from VS Code
        position <- PointF(x, y)

    member this.setColor(clr: Color) : unit =
        ignore this // Required to silence the "unused variable" warning from VS Code
        color <- clr

    member this.writeToFile() : unit =
        ignore this // Required to silence the "unused variable" warning from VS Code
        Console.ForegroundColor <- ConsoleColor.Green
        let filePath: string = "doodle.png"
        printfn "Doodle saved to '%s'." filePath

        if OperatingSystem.IsWindowsVersionAtLeast(6, 1) then
            canvas.Save filePath

        Console.ResetColor()


let colorFromHsv (h: float, s: float, v: float) : Color =
    let normalizedH: float =
        let raw: float = h - floor (abs h) * if h < 0.0 then -1.0 else 1.0
        if raw < 0.0 then raw + 1.0 else raw % 1.0

    let i: int = floor (normalizedH * 6.0) |> int
    let f: float = normalizedH * 6.0 - float i
    let p: float = v * (1.0 - s)
    let q: float = v * (1.0 - f * s)
    let t: float = v * (1.0 - (1.0 - f) * s)

    let (r: float, g: float, b: float) =
        match i % 6 with
        | 0 -> v, t, p
        | 1 -> q, v, p
        | 2 -> p, v, t
        | 3 -> p, q, v
        | 4 -> t, p, v
        | 5 -> v, p, q
        | _ -> 0.0, 0.0, 0.0 // Compiler requires exhaustive matching

    Color.FromArgb(255, int (r * 255.0), int (g * 255.0), int (b * 255.0))


[<EntryPoint>]
let main (args: string array) =
    ignore args // Required to silence the "unused variable" warning from VS Code

    let rnd: Random = Random()
    let mutable iterations: int16 = 0s
    Console.Clear()
    Console.ForegroundColor <- ConsoleColor.Cyan
    printfn "Welcome to the Doodle Generator!"
    printfn "The default number of iterations is 1000 when no input is provided."
    Console.ResetColor()

    printf "Enter the number of iterations: "
    let userInput: string = Console.ReadLine()

    try
        iterations <- Int16.Parse userInput
    with _ ->
        printfn
            "%s. Number of iterations defaults to 1000."
            (if userInput.Length = 0 then
                 "No input provided"
             else
                 "Invalid input")

        iterations <- 1000s

    let mutable hue: float = 0.0

    if OperatingSystem.IsWindowsVersionAtLeast(6, 1) then
        let turtle: MyTurtle = MyTurtle()

        for _ in 1 .. int iterations do
            (hue, 1.0, 1.0) |> colorFromHsv |> turtle.setColor
            rnd.Next(10, 50) |> float32 |> turtle.forward
            rnd.Next(-90, 90) |> float32 |> turtle.right
            hue <- hue + 0.005

            if hue >= 1.0 then
                hue <- 0.0

        turtle.writeToFile ()

    0
