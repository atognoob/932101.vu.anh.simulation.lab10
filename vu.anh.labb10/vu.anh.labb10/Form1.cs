using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vu.anh.labb10
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] teams = new string[8] { "ManU", "ManC", "Liverpool", "Chelsea", "Arsenal", "Tottenham", "Everton", "Fullham" };
        int[] teams_id = new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
        float[] team_lamda = new float[8];
        int[,] team_matches = new int[8, 8];
        int rounds = 1;

        Dictionary<string, Dictionary<string, int>> team_stat_name = new Dictionary<string, Dictionary<string, int>>();
        private void main_diagonale_setter()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (i == j) team_matches[i, j] = 1;
        }
        private int match(double lamda)
        {
            int x = 0;
            double sum = 0;
            Random rnd = new Random();
            while (sum > -lamda)
            {
                sum += Math.Log(rnd.NextDouble());
                x++;
            }
            x -= 1;
            return x;
        }
        private int[] teams_next_round_matches()
        {
            int[] teams_id_next = new int[8];
            int k = 0;
            int[] teams_temp = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i<8; i++)
            {
                if (teams_temp[i] ==0)
                    for(int j= 0;j<8; j++)
                    {
                        if (team_matches[i, j] !=1 && teams_temp[j] == 0)
                        {
                            teams_id_next[k] = i;
                            teams_id_next[k+1] = j;
                            k += 2;
                            team_matches[i, j] = 1;
                            teams_temp[i] = 1;
                            teams_temp[j] = 1;
                            break;
                        }
                    }
            }
            return teams_id_next;
        }
        private void sort()
        {
            team_stat_name = team_stat_name.OrderByDescending(pair => pair.Value["point"]).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button2.Visible = true;
            button1.Visible = false;
            button3.Visible = true;
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();
            main_diagonale_setter();
            Random rnd = new Random();
            for (int i =0;i< teams.Length; i++)
            {
                team_lamda[i] = (float)rnd.Next(501) / 100;
            }
            for (int i = 0; i < teams.Length; i++)
            {
                dataGridView1.Columns.Add(new DataGridViewColumn { HeaderText = teams[i], CellTemplate = new DataGridViewTextBoxCell() });
            }
            for (int i = 0;i < teams.Length; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].HeaderCell = new DataGridViewRowHeaderCell { Value = teams[i] };
            }
            for (int i = 0; i < teams.Length; i++)
                for (int j = 0; j < teams.Length; j++)
                    if (i == j) dataGridView1[i, j].Value = "---";
            for (int i = 0; i < teams.Length; i++)
            {
                team_stat_name[teams[i]] = new Dictionary<string, int>()
                {
                    ["point"] = 0,
                    ["win"] = 0,
                    ["draw"] = 0,
                    ["lose"] = 0
                };
            }
            foreach (var team in team_stat_name)
            {
                dataGridView2.Rows.Add(team.Key, team.Value["point"], team.Value["win"], team.Value["draw"], team.Value["lose"]);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView2.ClearSelection();
            for (int i = 0; i < teams.Length; i++)
                for (int j = 0; j < teams.Length; j++)
                    dataGridView1[i, j].Style.BackColor = Color.White;
            Random rnd = new Random();
            int[] next_round = new int[8];
            next_round = teams_next_round_matches();
            for (int i = 0; i < teams.Length; i += 2)
            {
                int a = next_round[i];
                int b = next_round[i + 1];
                int goals_a = match(team_lamda[a]);
                int goals_b = match(team_lamda[b]);

                if (goals_a == goals_b)
                {
                    team_stat_name[teams[a]]["point"] += 1;
                    team_stat_name[teams[a]]["draw"] += 1;
                    team_stat_name[teams[b]]["point"] += 1;
                    team_stat_name[teams[b]]["draw"] += 1;
                }
                else if (goals_a > goals_b)
                {
                    team_stat_name[teams[a]]["point"] += 3;
                    team_stat_name[teams[a]]["win"] += 1;
                    team_stat_name[teams[b]]["lose"] += 1;
                }
                else
                {
                    team_stat_name[teams[a]]["lose"] += 1;
                    team_stat_name[teams[b]]["point"] += 3;
                    team_stat_name[teams[b]]["win"] += 1;
                }

                dataGridView1[b, a].Value = $"{goals_a} - {goals_b}";
                dataGridView1[b, a].Style.BackColor = Color.Green;
            }
            sort();
            dataGridView2.Rows.Clear();
            foreach (var team in team_stat_name)
            {
                dataGridView2.Rows.Add(team.Key, team.Value["point"], team.Value["win"], team.Value["draw"], team.Value["lose"]);
            }

            if (rounds == 14)
            {
                
                dataGridView2.Rows[7].DefaultCellStyle.BackColor = Color.Gold;
                dataGridView2.Rows[6].DefaultCellStyle.BackColor = Color.Silver;
                dataGridView2.Rows[5].DefaultCellStyle.BackColor = Color.Brown;
            }
            rounds += 1;
            dataGridView2.ClearSelection();
            dataGridView2.ClearSelection();
            dataGridView2.ClearSelection();
            if (rounds == 15)
            {
                button1.Visible = false;
                button3.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
}
