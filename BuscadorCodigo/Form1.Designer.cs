namespace GeneradorInvestigacionApp;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        txtTema = new TextBox();
        btnGenerar = new Button();
        rtbResultado = new RichTextBox();
        SuspendLayout();
        // 
        // txtTema
        // 
        txtTema.Location = new Point(12, 12);
        txtTema.Name = "txtTema";
        txtTema.Size = new Size(600, 23);
        txtTema.TabIndex = 0;
        // 
        // btnGenerar
        // 
        btnGenerar.Location = new Point(630, 12);
        btnGenerar.Name = "btnGenerar";
        btnGenerar.Size = new Size(150, 23);
        btnGenerar.TabIndex = 1;
        btnGenerar.Text = "Generar";
        btnGenerar.UseVisualStyleBackColor = true;
        btnGenerar.Click += btnGenerar_Click;
        // 
        // rtbResultado
        // 
        rtbResultado.Location = new Point(12, 50);
        rtbResultado.Name = "rtbResultado";
        rtbResultado.Size = new Size(768, 350);
        rtbResultado.TabIndex = 5;
        rtbResultado.Text = "";
        rtbResultado.TextChanged += rtbResultado_TextChanged;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(txtTema);
        Controls.Add(btnGenerar);
        Controls.Add(rtbResultado);
        Name = "Form1";
        Text = "Generador de Investigación Académica";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TextBox txtTema;
    private System.Windows.Forms.Button btnGenerar;
    private System.Windows.Forms.RichTextBox rtbResultado;
}
