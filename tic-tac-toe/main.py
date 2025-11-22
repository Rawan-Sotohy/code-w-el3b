import tkinter as tk
from tkinter import messagebox
import random
import time

class TicTacToe:
    def __init__(self, root):
        self.root = root
        root.title("Tic Tac Toe")
        root.resizable(False, False)

        # Player choice will be set at start
        self.player = None
        self.ai = None

        self.bg = "#0f172a"
        root.configure(bg=self.bg)

        self.show_choice_screen()


    #       SCREEN 1 — Choose X or O
    
    def show_choice_screen(self):
        frame = tk.Frame(self.root, bg=self.bg)
        frame.pack(pady=40)

        title = tk.Label(frame, text="Choose Your Symbol", 
                         font=("Helvetica", 24, "bold"), fg="#38bdf8", bg=self.bg)
        title.pack(pady=20)

        btn_x = tk.Button(frame, text="Play as X", font=("Arial", 16, "bold"),
                          bg="#1e293b", fg="white", width=12, height=2,
                          command=lambda: self.start_game("X"))
        btn_x.pack(pady=10)

        btn_o = tk.Button(frame, text="Play as O", font=("Arial", 16, "bold"),
                          bg="#1e293b", fg="white", width=12, height=2,
                          command=lambda: self.start_game("O"))
        btn_o.pack(pady=10)

        self.choice_frame = frame


    #       Start Game After Choosing
    
    def start_game(self, choice):
        self.player = choice
        self.ai = "O" if choice == "X" else "X"

        self.choice_frame.destroy()

        self.buttons = {}
        self.board = [["" for _ in range(3)] for _ in range(3)]
        self.game_over = False

        self.cell_bg = "#1e293b"
        self.cell_hover = "#334155"
        self.player_color = "#4ade80"  # green
        self.ai_color = "#f87171"      # red
        self.win_glow = "#38bdf8"      # blue/cyan

        self.build_ui()

        # If player chose O, AI starts
        if self.player == "O":
            self.root.after(300, self.ai_move)


    #       Build Game UI
    
    def build_ui(self):
        title = tk.Label(self.root, text="Tic Tac Toe", font=("Helvetica", 24, "bold"),
                         fg="#38bdf8", bg=self.bg)
        title.pack(pady=10)

        # Info label removed as requested
        # self.info_label = tk.Label(...)

        grid_frame = tk.Frame(self.root, bg=self.bg)
        grid_frame.pack(pady=10)

        for r in range(3):
            for c in range(3):
                b = tk.Label(grid_frame, text="", font=("Arial", 38, "bold"),
                             bg=self.cell_bg, fg="white", width=4, height=2,
                             bd=2, relief="ridge")
                b.grid(row=r, column=c, padx=8, pady=8)

                b.bind("<Button-1>", lambda e, rr=r, cc=c: self.handle_click(rr, cc))
                b.bind("<Enter>", lambda e, btn=b: btn.config(bg=self.cell_hover))
                b.bind("<Leave>", lambda e, btn=b: btn.config(bg=self.cell_bg))

                self.buttons[(r, c)] = b

        reset_btn = tk.Button(self.root, text="Reset", command=self.reset_game,
                              font=("Arial", 12), bg="#475569", fg="white",
                              width=12)
        reset_btn.pack(pady=8)


    #       Handle Player Click
    
    def handle_click(self, r, c):
        if self.game_over or self.board[r][c] != "":
            return

        self.make_move(r, c, self.player)
        winner = self.check_winner()
        if winner or self.is_full():
            self.end_game(winner)
            return

        self.root.after(300, self.ai_move)


    #       Animate X/O Placement
    
    def animate_piece(self, btn, symbol, color):
        for size in range(10, 38, 3):
            btn.config(font=("Arial", size, "bold"), fg=color)
            btn.update()
            time.sleep(0.01)
        btn.config(text=symbol)


    #       Make a Move
    
    def make_move(self, r, c, who):
        self.board[r][c] = who
        btn = self.buttons[(r, c)]

        color = self.player_color if who == self.player else self.ai_color
        self.animate_piece(btn, who, color)

        btn.unbind("<Button-1>")
        btn.config(bg=self.cell_bg)


    #       AI Move
    
    def ai_move(self):
        if self.game_over:
            return

        empties = [(r, c) for r in range(3) for c in range(3)
                   if self.board[r][c] == ""]

        # Try to win
        for r, c in empties:
            self.board[r][c] = self.ai
            if self.check_winner() == self.ai:
                self.board[r][c] = ""
                self.make_move(r, c, self.ai)
                if self.check_winner():
                    self.end_game(self.ai)
                return
            self.board[r][c] = ""

        # Block player
        for r, c in empties:
            self.board[r][c] = self.player
            if self.check_winner() == self.player:
                self.board[r][c] = ""
                self.make_move(r, c, self.ai)
                return
            self.board[r][c] = ""

        # Otherwise random
        r, c = random.choice(empties)
        self.make_move(r, c, self.ai)

        winner = self.check_winner()
        if winner or self.is_full():
            self.end_game(winner)


    #       Check Winner
    
    def check_winner(self):
        b = self.board
        lines = [
            [b[0][0], b[0][1], b[0][2]],
            [b[1][0], b[1][1], b[1][2]],
            [b[2][0], b[2][1], b[2][2]],
            [b[0][0], b[1][0], b[2][0]],
            [b[0][1], b[1][1], b[2][1]],
            [b[0][2], b[1][2], b[2][2]],
            [b[0][0], b[1][1], b[2][2]],
            [b[0][2], b[1][1], b[2][0]],
        ]
        for line in lines:
            if line.count("X") == 3:
                return "X"
            if line.count("O") == 3:
                return "O"
        return None


    #       Check Full Board
    
    def is_full(self):
        return all(self.board[r][c] != "" for r in range(3) for c in range(3))


    #       Glow Winning Cells
    
    def glow_winner(self, cells):
        for _ in range(6):
            for r, c in cells:
                btn = self.buttons[(r, c)]
                btn.config(bg=self.win_glow)
            self.root.update()
            time.sleep(0.1)
            for r, c in cells:
                btn = self.buttons[(r, c)]
                btn.config(bg=self.cell_bg)
            self.root.update()
            time.sleep(0.1)


    #       End Game
    
    def end_game(self, winner):
        self.game_over = True

        if winner == self.player:
            cells = self.get_winning_cells()
            self.glow_winner(cells)
            messagebox.showinfo("Winner", "You Win!")
        elif winner == self.ai:
            cells = self.get_winning_cells()
            self.glow_winner(cells)
            messagebox.showinfo("AI Win", "AI Wins. Try again!")
        else:
            messagebox.showinfo("Draw", "It's a Draw!")

        for btn in self.buttons.values():
            btn.unbind("<Button-1>")

    #       Get Winning Cells

    def get_winning_cells(self):
        b = self.board
        wins = [
            [(0,0),(0,1),(0,2)],
            [(1,0),(1,1),(1,2)],
            [(2,0),(2,1),(2,2)],
            [(0,0),(1,0),(2,0)],
            [(0,1),(1,1),(2,1)],
            [(0,2),(1,2),(2,2)],
            [(0,0),(1,1),(2,2)],
            [(0,2),(1,1),(2,0)],
        ]
        for line in wins:
            r1,c1 = line[0]
            r2,c2 = line[1]
            r3,c3 = line[2]
            if b[r1][c1] == b[r2][c2] == b[r3][c3] != "":
                return line
        return []

    #       Reset Game
    
    def reset_game(self):
        for widget in self.root.winfo_children():
            widget.destroy()
        self.__init__(self.root)


if __name__ == "__main__":
    root = tk.Tk()
    app = TicTacToe(root)
    root.mainloop()
