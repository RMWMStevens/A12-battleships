using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Battleships
{
    public partial class Battleships : Form
    {
        public Battleships()
        {
            InitializeComponent();
        }

        // MediaPlayer voor de Hit-geluiden. sA = TableA (links), sB = TableB (rechts)
        WMPLib.WindowsMediaPlayer sA = new WMPLib.WindowsMediaPlayer();
        WMPLib.WindowsMediaPlayer sB = new WMPLib.WindowsMediaPlayer();
        // MediaPlayer voor de Main Theme
        WMPLib.WindowsMediaPlayer sSong = new WMPLib.WindowsMediaPlayer();

        // List<PictureBox> TableA = new List<PictureBox>();
        PictureBox[] TableA = new PictureBox[100];
        PictureBox[] TableB = new PictureBox[100];

        // Hit-List waar de laatst geraakte schepen in opgeslagen worden. Deze wordt gebruikt door de PC-Player
        List<int> HitHistory = new List<int>();

        // Battleships
        int iAirplane = 1;
        int iBattleship = 3;
        int iPatrol = 3;

        // Deze variable neemt de lengte van het schip over uit PlaceShip() voor Ink[color](), DrawPath()
        int iShipSize = 0;
        // Afhankelijk van de waarde van deze Int, legt het spel het juiste pad af.
        int iSHIPS = 0;
        // Values: 0 = Nothing / 1 = Airplane / 2 = Battleship / 3 = Patrol / 4 = Airplane EDIT / 5 = Battleship EDIT / 6 = Patrol EDIT

        // Score
        int iMissesA = 0;
        int iMissesB = 0;
        int iHitsA = 0;
        int iHitsB = 0;
        // Bij True hebben sommige functies een ander doel dan bij StartGame = false;
        bool StartGame = false;

        // Omdat het soms nog niet goed gaat met schepen neerleggen is dit een "last resort" om de Speler toch te kunnen laten winnen
        // Edited: int iMaxHits = 0;

        // Delay
        int iDelay = 0;

        // Procesgegevens

        // Houdt bij op welke PicBox de gebruiker heeft geklikt
        int iInUse = 0;

        // Booleans om richtingen te proberen voor de Linkerkant
        bool bRight = false;
        bool bLeft = false;
        bool bUp = false;
        bool bDown = false;

        // Booleans om richtingen te proberen voor de Rechterkant
        bool bRightB = false;
        bool bLeftB = false;
        bool bUpB = false;
        bool bDownB = false;

        // When true, sound effects and animations play
        bool SpecialEffects = true;

        // Globale Int om de random waarde in op te slaan
        int iRandomNumber = 0;

        // Declaratie MainMenu
        MainMenu mainMenu;

        private void Battleships_Load(object sender, EventArgs e)
        {
            // Tijdelijke List om alle PictureBox controls in te laden die een 'Tag' hebben
            List<PictureBox> tempTableA = new List<PictureBox>();
            List<PictureBox> tempTableB = new List<PictureBox>();

            // Vindt alle controls op het Form
            foreach (Control x in this.Controls)
            {
                // Filtert of Pictureboxes die *niet* GEEN 'Tag' hebben
                if (x is PictureBox && x.Tag != null)
                {
                    // Wanneer de 'Tag' gelijk is aan "A", wordt de picturebox toegevoegd aan tempTableA
                    if (x.Tag.ToString() == "A")
                    {
                        tempTableA.Add((PictureBox)x);
                    }
                    // Wanneer de 'Tag' gelijk is aan "B", tempTableB
                    if (x.Tag.ToString() == "B")
                    {
                        tempTableB.Add((PictureBox)x);
                    }
                }
            }

            // 2 strings om de namen van de PictureBoxes mee uit te lezen
            string box;
            string cutbox;

            // Aangezien er 100 PictureBoxes zijn per Table, een For-loop met 100 'stapjes'
            for (int i = 0; i < 100; i++)
            {
                // Eerst wordt de naam van de eerste entry van TempTableA uitgelezen en opgeslagen in string "Box"
                box = tempTableA[i].Name.ToString();
                // Dan wordt er een substring genomen zodat het "pbA"-deel wegvalt en de getallen overblijven
                cutbox = box.Substring(3);
                // Plek "cutbox"-1 in TableA krijgt de waarde van 'i' in de For-loop
                // Voorbeeld: PictureBox "pbA20" op plek 35 in de 'tempTableA' wordt bijgesneden tot "20"
                //            TableA[20-1] = tempTable[35];
                //            TableA[19]   = tempTable[35];
                TableA[int.Parse(cutbox) - 1] = tempTableA[i];
                // Op de bovenstaande manier worden de PictureBoxes 1 t/m 100 op plekken 0 t/m 99 geplaatst op volgorde

                // Toelichting hierboven
                box = tempTableB[i].Name.ToString();
                cutbox = box.Substring(3);
                TableB[int.Parse(cutbox) - 1] = tempTableB[i];
            }

            //// Foto's weghalen die de pictureboxes bedekken. Dit was nodig om te controleren of de PC niet twee boten op elkaar legde.
            //for (int i = 0; i < 100; i++)
            //{
            //    TableB[i].BackgroundImage = null;
            //}

            // Functie PickMusic kiest een 'random' liedje uit de "sounds/music"-folder en laadt deze in de Windows Media Player via URL. Spelen begint nog niet.
            PickMusic();
        }

        private void PlaceShip(object sender, EventArgs e)
        {
            // Genereer een nieuw Random Getal voor TableB
            GenNum();

            // Er wordt gekeken welke knop ervoor gezorgd heeft dat deze functie aangeroepen werd
            Button btn = (Button)sender;
            // De "name" van deze knop wordt gebruikt voor de switch-case hieronder
            string name = btn.Name;

            switch (name)
            {
                // Wanneer het een Airplane Carrier is, moet:
                case "btnAirplane":
                    // ... iSHIPS pad 1 volgen...
                    iSHIPS = 1;
                    // ... met een bootlengte van t/m +5 (dus 6) blokjes
                    iShipSize = 5;
                    break;
                // Wanneer het een Battleships is, moet:
                case "btnBattleship":
                    // ... iSHIPS pad 2 volgen...
                    iSHIPS = 2;
                    // ... met een bootlengte van t/m +3 (dus 4) blokjes
                    iShipSize = 3;
                    break;
                // Wanneer het een Patrolboat is, moet...
                case "btnPatrol":
                    // ... iSHIPS pad 3 volgen...
                    iSHIPS = 3;
                    // ... met een bootlengte van t/m +2 (dus 3) blokjes
                    iShipSize = 2;
                    break;
            }

            // Keuze-specifieke meldingen tonen bij een exception
            switch (iSHIPS)
            {
                case 1:
                    // Wanneer er meer dan één Airplane Carrier beschikbaar is, kan het veld wit worden om de schepen te tekenen. Etc.
                    if (iAirplane > 0)
                    {
                        InkWhite();
                    }
                    // Zo niet, volgt er een MessageBox met de melding dat er geen schepen van dat type meer beschikbaar zijn. iSHIPS wordt weer 0. Etc.
                    else
                    {
                        MessageBox.Show("No more Airplane Carriers available!", "Out of Ships");
                        iSHIPS = 0;
                    }
                    break;
                case 2:
                    if (iBattleship > 0)
                    {
                        InkWhite();
                    }
                    else
                    {
                        MessageBox.Show("No more Battleships available!", "Out of Ships");
                        iSHIPS = 0;
                    }
                    break;
                case 3:
                    if (iPatrol > 0)
                    {
                        InkWhite();
                    }
                    else
                    {
                        MessageBox.Show("No more Patrolboats available!", "Out of Ships");
                        iSHIPS = 0;
                    }
                    break;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Wanneer de BackColor van de Button niet zwart is, ga door met de code
            // Deze is alleen zwart wanneer er gewonnen is door de PC of door de speler. De knoppen wisselen van kleur om aan te duiden dat het spel over is, en "Start" wordt zwart
            if (btnStart.BackColor != Color.Black)
            {
                // Wanneer de gebruiker wilt starten zonder dat alle boten geplaatst zijn, volgt er een foutmelding
                if (iAirplane != 0 || iPatrol != 0 || iBattleship != 0)
                {
                    MessageBox.Show("You still have ships to place! Place these first before clicking START", "Place your Ships!", MessageBoxButtons.OK);
                }
                // Wanneer wel alle boten op het speelveld liggen, wordt het spel gestart.
                else
                {
                    // Edited: Wat hieronder staat is niet langer van toepassing, misschien later weer
                    // De Int "iMaxHits" krijgt een waarde 1:1 op het aantal zwarte blokjes geplaatst door de 'bot'. Dit is omdat eerder sommige schepen over elkaar kwamen te liggen,
                    // dit probleem is nu verholpen maar voor de zekerheid heb ik het er in laten zitten, mocht het toch nog een keer fout gaan. Iedere keer als er een schip geraakt wordt
                    // op het speelveld van de tegenstander, iMaxHits--. Bij iMaxHits == 0 heeft de Player gewonnen.
                    //for (int i = 0; i < 100; i++)
                    //{
                    //    // Als de achtergrondkleur zwart is, verhoog iMaxHits met één
                    //    if (TableB[i].BackColor == Color.Black)
                    //    {
                    //        iMaxHits++;
                    //    }
                    //}

                    // De boolean StartGame gaat op "True" zodat sommige functions andere acties uitvoeren.
                    StartGame = true;

                    // Scheepplaatsfunctionaliteit wordt ge-disabled
                    btnAirplane.Enabled = false;
                    btnBattleship.Enabled = false;
                    btnPatrol.Enabled = false;
                    txtAirplane.Enabled = false;
                    txtBattleship.Enabled = false;
                    txtPatrol.Enabled = false;

                    // ButtonStart wordt uitgeschakeld en krijgt een duidelijke "Inactive" kleur
                    btnStart.Enabled = false;
                    btnStart.BackColor = SystemColors.Control;

                    // Aangezien PlayerA mag beginnen, wordt de achtergrond van zijn Label groen van kleur.
                    lblPlayerA.BackColor = Color.LightGreen;

                    // Wanneer de Checkbox voor muziek aangevinkt is, start de muziek te spelen.
                    if (chkMusic.Checked)
                    {
                        sSong.controls.play();
                    }

                    // Wanneer de Boolean "Special Effects" 'True' is, speelt het 'startGame'-muziekje.
                    // Deze boolean bepaalt of animaties afspelen en of er soundeffects zijn bij het raken en missen van schepen door beide partijen.
                    if (SpecialEffects)
                    {
                        sA.URL = "sounds/startgame.mp3";
                    }
                }
            }
            else
            {
                // Wanneer de achtergrondkleur van de knop *WEL* zwart is, stuur terug naar functie WinLoss.
                // Dit is omdat de knop "btnStart" eventjes een andere functie heeft wanneer er gewonnen of verloren is door één van de 2 spelers
                // De "btnStart" is dan een Toggle om de feliciteertekst + afbeelding te verbergen / tonen
                WinLoss();
            }
        }

        // Deze knop spreekt natuurlijk voor zich.
        private void btnReset_Click(object sender, EventArgs e)
        {
            // De muziek wordt gestopt
            sSong.controls.stop();
            // ... en er wordt direct een ander nummer gekozen voor wanneer er nog een volgend spel begint.
            PickMusic();

            StartGame = false;

            // Alle PictureBoxes krijgen de texture "txWater" en een BackColor van "DeepSkyBlue"
            // Ook de 'Tag' wordt aangepast, dit omdat er m.b.v. de 'Tag' bepaald wordt of het vakje al eerder is geraakt of niet.
            for (int i = 0; i < 100; i++)
            {
                TableA[i].BackgroundImage = Properties.Resources.txWater;
                TableA[i].BackColor = Color.DeepSkyBlue;
                TableB[i].BackColor = Color.DeepSkyBlue;
                TableB[i].BackgroundImage = Properties.Resources.txWater;
                TableA[i].Tag = "0";
                TableB[i].Tag = "0";
            }

            // Alle controls terugzetten als hoe ze waren voordat het eerste spel gestart werd

            btnAirplane.Enabled = true;
            btnBattleship.Enabled = true;
            btnPatrol.Enabled = true;
            txtAirplane.Enabled = true;
            txtBattleship.Enabled = true;
            txtPatrol.Enabled = true;

            btnStart.Enabled = true;

            btnStart.BackColor = Color.LimeGreen;
            btnStart.ForeColor = Color.Black;
            btnStart.Text = "START";
            btnReset.BackColor = Color.Red;
            btnReset.BackColor = Color.Red;
            btnReset.Text = "RESET";

            // Alle variabelen terugzetten als hoe ze waren voordat het eerste spel gestart werd.

            iInUse = 0;

            iAirplane = 1;
            iBattleship = 3;
            iPatrol = 3;

            iMissesA = 0;
            iMissesB = 0;
            iHitsA = 0;
            iHitsB = 0;

            StartGame = false;

            lblWinLoss.Visible = false;
            pbWinLoss.Visible = false;

            // StatUpdate refresht alle textboxen om de juiste hoeveelheden schepen weer te tonen.
            StatUpdate();
        }

        private void Calculate()
        {
            // Wanneer de game nog niet is gestart, bepaalt Calculate() waar de schepen heen kunnen voor beide spelers
            if (!StartGame)
            {
                // Omdat variabelen veranderd moeten worden, heb ik ze in aparte, lokale variabelen gestopt
                int iProc = iInUse;
                int iSize = iShipSize;
                int iRandom = iRandomNumber;

                // Omdat er bij "UP" en "DOWN" met stappen van 10 gewerkt wordt (één regel is 10 lang, van plek 25 één omhoog = plek 15), worden deze
                //   als eerst gedaan. Later wordt "LEFT" en "RIGHT" bekeken, dan wordt het getal uitgedund tot een getal van 0-9

                // A-kant "DOWN" bepaalt of er, vanaf de geselecteerde Picturebox een lijn naar beneden getrokken kan worden...
                if (iProc + (iSize * 10) < 100)
                {
                    bDown = true;
                }
                // ... of dat deze buiten het speelveld valt
                else
                {
                    bDown = false;
                }

                // B-kant "DOWN" doet exact hetzelfde als A, alleen wordt er nu gekeken of er vanaf het random gegenereerde getal een pad getrokken kan worden
                if (iRandom + (iSize * 10) < 100)
                {
                    bDownB = true;
                }
                else
                {
                    bDownB = false;
                }

                // A "UP"
                if (iProc - (iSize * 10) >= 0)
                {
                    bUp = true;
                }
                else
                {
                    bUp = false;
                }

                // B "UP"
                if (iRandom - (iSize * 10) >= 0)
                {
                    bUpB = true;
                }
                else
                {
                    bUpB = false;
                }


                // Aangeklikte PictureBox uitdunnen tot een 1-cijferig getal zodat "LEFT" en "RIGHT" geprobeerd kunnen worden
                while (iProc >= 10)
                {
                    iProc -= 10;
                }

                // Random Picturebox uitdunnen
                while (iRandom >= 10)
                {
                    iRandom -= 10;
                }

                // A "RIGHT"
                if ((iProc + iSize) < 10)
                {
                    bRight = true;
                }
                else
                {
                    bRight = false;
                }

                // B "LEFT"
                if ((iRandom + iSize) < 10)
                {
                    bRightB = true;
                }
                else
                {
                    bRightB = false;
                }

                // A "LEFT"
                if ((iProc - iSize) >= 0)
                {
                    bLeft = true;
                }
                else
                {
                    bLeft = false;
                }

                // B "LEFT"
                if ((iRandom - iSize) >= 0)
                {
                    bLeftB = true;
                }
                else
                {
                    bLeftB = false;
                }

                // Een aparte functie, zogenaamd de "BlackChecker" omdat deze controleert of één van de mogelijke richtingen al een zwart blokje bevat
                // Als dit niet gecontroleert zou worden, krijg je steeds boten die over elkaar heen liggen. Dit kan natuurlijk niet.

                // A-SIDE "DOWN" Wanneer boolean "Down" 'true' is, kijkt de "BlackChecker" of er een zwart blokje tussen zit
                if (bDown)
                {
                    for (int i = iInUse; i <= (iInUse + (iShipSize * 10)); i += 10)
                    {
                        // Zo ja...
                        if (TableA[i].BackColor == Color.Black)
                        {
                            // ... dan wordt de boolean alsnog teruggezet naar 'false'
                            bDown = false;
                        }
                    }
                }

                if (bUp)
                {
                    for (int i = iInUse; i >= (iInUse - (iShipSize * 10)); i -= 10)
                    {
                        if (TableA[i].BackColor == Color.Black)
                        {
                            bUp = false;
                        }
                    }
                }

                if (bLeft)
                {
                    for (int i = iInUse; i >= (iInUse - iShipSize); i--)
                    {
                        if (TableA[i].BackColor == Color.Black)
                        {
                            bLeft = false;
                        }
                    }
                }

                if (bRight)
                {
                    for (int i = iInUse; i <= (iInUse + iShipSize); i++)
                    {
                        if (TableA[i].BackColor == Color.Black)
                        {
                            bRight = false;
                        }
                    }
                }

                // Natuurlijk moet dit ook gebeuren voor de B-kant (rechterkant). Zelfde principe als hierboven.
                // B-SIDE
                if (bDownB)
                {
                    for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                    {
                        if (TableB[i].BackColor == Color.Black)
                        {
                            bDownB = false;
                        }
                    }
                }

                if (bUpB)
                {
                    for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                    {
                        if (TableB[i].BackColor == Color.Black)
                        {
                            bUpB = false;
                        }
                    }
                }

                if (bLeftB)
                {
                    for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                    {
                        if (TableB[i].BackColor == Color.Black)
                        {
                            bLeftB = false;
                        }
                    }
                }

                if (bRightB)
                {
                    for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                    {
                        if (TableB[i].BackColor == Color.Black)
                        {
                            bRightB = false;
                        }
                    }
                }
            }

        }

        // De functie "InkWhite()" kleurt alle blokjes die *NIET* zwart zijn, wit. Ook worden de kleine textures van de PictureBoxes afgehaald zodat de kleuren gezien kunnen worden
        private void InkWhite()
        {
            for (int i = 0; i < 100; i++)
            {
                // Alleen wanneer de kleur niet zwart is, krijgt de PictureBox een witte kleur. Dit is een prikkel naar de speler om indirect te zeggen: "Klik hier ergens!"
                if (TableA[i].BackColor != Color.Black)
                {
                    TableA[i].BackgroundImage = null;
                    TableA[i].BackColor = Color.White;
                }
            }
        }

        // De functie "InkBlue()" doet exact hetzelfde als "InkWhite()", alleen wordt de achtergrondkleur weer blauw en komt de watertexture terug op de PictureBox
        private void InkBlue()
        {
            for (int i = 0; i < 100; i++)
            {
                if (TableA[i].BackColor != Color.Black)
                {
                    TableA[i].BackgroundImage = Properties.Resources.txWater;
                    TableA[i].BackColor = Color.Blue;
                }
            }
        }

        // De functie "InkPath()" tekent een lijn in de richtingen die 'goedgekeurd' zijn door "Calculate()". Er wordt een witte lijn getrokken op de plekken waar de boot *kan* komen te liggen,
        // wanneer de gebruiker op één van die witte tinten aan het einde van de lijn klikt, wordt zijn keuze door "DrawPath()" (hieronder) vastgelegd in het zwart.
        private void InkPath()
        {
            if (bRight) // FLORALWHITE als laatste vakje
            {
                for (int i = iInUse; i <= (iInUse + iShipSize); i++)
                {
                    TableA[i].BackgroundImage = null;
                    TableA[i].BackColor = Color.LightGray;
                }
                TableA[iInUse + iShipSize].BackColor = Color.FloralWhite;
            }

            if (bLeft) // GHOSTWHITE als laatste vakje
            {
                for (int i = iInUse; i >= (iInUse - iShipSize); i--)
                {
                    TableA[i].BackgroundImage = null;
                    TableA[i].BackColor = Color.LightGray;
                }
                TableA[iInUse - iShipSize].BackColor = Color.GhostWhite;
            }

            if (bUp) // WHITE als laatste vakje
            {
                for (int i = iInUse; i >= (iInUse - (iShipSize * 10)); i -= 10)
                {
                    TableA[i].BackgroundImage = null;
                    TableA[i].BackColor = Color.LightGray;
                }
                TableA[iInUse - (10 * iShipSize)].BackColor = Color.White;
            }

            if (bDown) // WHITESMOKE als laatste vakje
            {
                for (int i = iInUse; i <= (iInUse + (iShipSize * 10)); i += 10)
                {
                    TableA[i].BackgroundImage = null;
                    TableA[i].BackColor = Color.LightGray;
                }
                TableA[iInUse + (10 * iShipSize)].BackColor = Color.WhiteSmoke;
            }

        }

        // De functie "DrawPath()" maakt van een witte 'voorbeeldlijn' van "InkPath()" hierboven een definitieve, zwarte print.
        // Er worden verschillende tinten wit gebruikt om aan te kunnen duiden welke richting geselecteerd is. 
        // "bRight" heeft tint 'FloralWhite', "bLeft" 'GhostWhite', "bUp" 'White', "bDown" 'WhiteSmoke'
        private void DrawPath()
        {
            // Voorbeeld:
            // Lengte boot = 3, van 22 naar 20
            // 22 is iInUse 1, 20 is iInUse 2
            // de lijn moet van 20 naar 22 getekend worden = left
            // van 42 naar 22 = down
            // van 24 naar 22 = right
            // van 2 naar 22 = up
            // Down, Left, Up & Right == false

            // DOWN
            if (TableA[iInUse].BackColor == Color.WhiteSmoke)
            {
                for (int i = iInUse; i >= (iInUse - (iShipSize * 10)); i -= 10)
                {
                    TableA[i].BackColor = Color.Black;
                }
            }

            // int i = 42; 42 > (42 - 3*10); 42 - 10
            // 42, 42 > 12; 42-10
            // 42, 32, 22, 12x = 3 stappen

            // UP
            if (TableA[iInUse].BackColor == Color.White)
            {
                for (int i = iInUse; i <= (iInUse + (iShipSize * 10)); i += 10)
                {
                    TableA[i].BackColor = Color.Black;
                }
            }

            // Aantekeningen berekening
            // int i = 2; 2 < (2 + 3*10); 2 + 10
            // int i = 2; 2 < 32; 2 + 10
            // 2, 12, 22, 32x = 3 stappen

            // LEFT
            if (TableA[iInUse].BackColor == Color.GhostWhite)
            {
                for (int i = iInUse; i <= (iInUse + iShipSize); i++)
                {
                    TableA[i].BackColor = Color.Black;
                }
            }

            // RIGHT
            if (TableA[iInUse].BackColor == Color.FloralWhite)
            {
                for (int i = iInUse; i >= (iInUse - iShipSize); i--)
                {
                    TableA[i].BackColor = Color.Black;
                }
            }

            // Schip aftrekken

            switch (iSHIPS)
            {
                case 4:
                    iAirplane -= 1;
                    break;
                case 5:
                    iBattleship -= 1;
                    break;
                case 6:
                    iPatrol -= 1;
                    break;
            }

            iSHIPS = 0;
            // Plaats naar nieuwe functie

            // Na het zwartmaken van de gewenste lijn, de overige witte strepen terugveranderen naar blauw. 
            InkBlue();
            // Een schip genereren aan de andere kant, dus in TableB
            GenerateBShip();
        }

        // StatUpdate gooit de ints in de tekstvakken... That's about it!
        private void StatUpdate()
        {
            txtAirplane.Text = iAirplane.ToString();
            txtBattleship.Text = iBattleship.ToString();
            txtPatrol.Text = iPatrol.ToString();
            lblMissesA.Text = iMissesA.ToString();
            lblMissesB.Text = iMissesB.ToString();
            lblHitsA.Text = iHitsA.ToString();
            lblHitsB.Text = iHitsB.ToString();
        }

        // "AClick" is het click-event voor alles wat er op TableA gebeurt (dus het plaatsen van schepen, tijdens het spel klikt de gebruiker slechts op TableB)
        private void AClick(object sender, EventArgs e)
        {
            // Er wordt bepaald wie de afzender is
            PictureBox pix = (PictureBox)sender;
            string box = pix.Name;
            // Int iInUse krijgt de waarde van de naam van de PictureBox - 1 (aangezien de List "TableA" vanaf 0 begint en de PictureBoxes van 1)
            iInUse = int.Parse(box.Substring(3)) - 1;

            // Wanneer het spel nog niet gestart is, worden de boten nog neergelegd.
            if (!StartGame)
            {
                // "iSHIPS" werd hier gebruikt om boot-specifieke dingen te doen. Dit bleek later minder nodig maar ik laat de 'structuur' staan om in de toekomst misschien wat mee te doen (zie MoSCoW)
                switch (iSHIPS)
                {
                    case 0:
                        // Wanneer de achtergrondkleur van "btnStart" gelijk is aan "Color.Black", is het spel al beëndigd. De speler moet dan op "Reset" klikken als hij een nieuw potje wilt doen
                        if (btnStart.BackColor == Color.Black)
                        {
                            MessageBox.Show("You are currently in Spectator Mode! To play a new game, press RESET");
                        }
                        else
                        {
                            // Wanneer de gebruiker geen Button aangeklikt heeft, maar toch op een PictureBox in TableA klikt.
                            MessageBox.Show("Please select a ship first!" + Environment.NewLine + Environment.NewLine + "This can also be done with the numbers 1, 2 and 3 on your keyboard!");
                        }
                        break;
                    case 1:
                        InkBlue();
                        Calculate();
                        InkPath();
                        iSHIPS = 4;
                        break;
                    case 2:
                        InkBlue();
                        Calculate();
                        InkPath();
                        iSHIPS = 5;
                        break;
                    case 3:
                        InkBlue();
                        Calculate();
                        InkPath();
                        iSHIPS = 6;
                        break;
                    case 4:
                        if (TableA[iInUse].BackColor == Color.White || TableA[iInUse].BackColor == Color.FloralWhite || TableA[iInUse].BackColor == Color.GhostWhite || TableA[iInUse].BackColor == Color.WhiteSmoke)
                        {
                            DrawPath();
                        }
                        break;
                    case 5:
                        if (TableA[iInUse].BackColor == Color.White || TableA[iInUse].BackColor == Color.FloralWhite || TableA[iInUse].BackColor == Color.GhostWhite || TableA[iInUse].BackColor == Color.WhiteSmoke)
                        {
                            DrawPath();
                        }
                        break;
                    case 6:
                        if (TableA[iInUse].BackColor == Color.White || TableA[iInUse].BackColor == Color.FloralWhite || TableA[iInUse].BackColor == Color.GhostWhite || TableA[iInUse].BackColor == Color.WhiteSmoke)
                        {
                            DrawPath();
                        }
                        break;
                }
            }
            // Wanneer het spel gestart is en de Speler klikt op TableA (dus het linker speelveld) krijgt hij de melding niet op zijn eigen boten te schieten
            else
            {
                MessageBox.Show("Don't try to shoot your own ships, dummy!", "Wrong Side");
            }

            // ... en natuurlijk de statistieken updaten
            StatUpdate();
        }

        // "BClick" is het click-event voor alles wat er op TableB gebeurt (het schieten op de schepen van PC-Player bijvoorbeeld)
        private void BClick(object sender, EventArgs e)
        {
            // Wanneer StartGame == true, moet de game controleren of er een schip is geraakt of niet.
            if (StartGame)
            {
                // Eerst kijken welke PictureBox geselecteerd is...
                PictureBox pix = (PictureBox)sender;
                // ... dan de naam daarvan pakken
                string box = pix.Name;
                // ... en dan het verkregen getal - 1 in de Int iInUse stoppen
                iInUse = int.Parse(box.Substring(3)) - 1;

                // Wanneer de achtergrond van TableB[iInUse] gelijk is aan de kleur Zwart, is het een hit
                if (TableB[iInUse].BackColor == Color.Black)
                {
                    // Een explosie-foto wordt op het schip geplaatst
                    TableB[iInUse].BackgroundImage = Properties.Resources.txExplosion;
                    // De kleur wordt veranderd naar donkerkrijs om het een duidelijkere kleur te geven naast het wijzigen van de afbeelding erbovenop
                    TableB[iInUse].BackColor = Color.DimGray;
                    // De "Hit-score" van Player A wordt verhoogd met één
                    iHitsA++;
                    // Omdat er net een 'hit' is geweest, moet "iMaxHits" met één verminderd worden. Nogmaals, bij 0 heeft SpelerA gewonnen
                    // Edited: iMaxHits--;
                    // De Tag van "TableB[iInUse] wordt gewijzigd naar "hit" om in de volgende "else-if" te gebruiken wanneer de achtergrond van "TableB[iInUse]" niet langer Color.Black is
                    TableB[iInUse].Tag = "hit";

                    // Wanneer "SpecialEffects" aan staat, speelt de animatie en een geluid
                    if (SpecialEffects)
                    {
                        // Eerst een "hit"-sound
                        sB.URL = "sounds/enemyhit.mp3";
                        // Dan wordt de Game gedisabled (zodat er niet 2x geschoten kan worden voordat de PC-Player kans krijgt om te schieten)
                        StartGame = false;
                        // De Timer zorgt voor de 'delay' tussen PlayerA & PC-Player's schot
                        tmrTimer.Enabled = true;

                        // De achtergrond van LabelA wordt grijs...
                        lblPlayerA.BackColor = SystemColors.Control;
                        // ... en de achtergrond van PlayerB (PC-Player) wordt lichtgroen om aan te tonen dat het niet jouw beurt is
                        lblPlayerB.BackColor = Color.LightGreen;
                    }
                    // Als "SpecialEffects" false is, wilt de speler dus geen animaties en soundeffects horen. In dat geval worden die geskipt en gaat de game meteen door naar EnemyShot(),
                    //   de functie die het schieten voor de computerkant doet
                    else
                    {
                        EnemyShot();
                    }
                }
                // De achtergrond is niet meer zwart, dus gaat hij door in de if-statement. Wanneer de 'Tag' gelijk is aan "Hit", probeert SpelerA een stuk schip nogmaals te raken. Dan volgt er er een foutmelding,
                //   en gaat het spel NIET door naar functie "EnemyShot()" tot er een schot geraakt of gemist is
                else if (TableB[iInUse].Tag.ToString() == "hit")
                {
                    MessageBox.Show("This part of the enemy's fleet has already been hit!", "Incorrect Target");
                }
                // Hetzelfde geldt voor 'Tag' == "Miss", alleen dan probeert de speler een al eerder beschoten leeg stuk water te raken
                else if (TableB[iInUse].Tag.ToString() == "miss")
                {
                    MessageBox.Show("Nothing seems to be here!", "Incorrect Target");
                }
                // Wanneer geen van de bovenstaande 'True' is, heeft de speler dus het schot gemist
                else
                {
                    // Deze Random() wordt gebruikt om een 'unieke' "Splash" te laten zien bij het missen. Er zijn er 3, zodat niet ieder gemist schot dezelfde texture krijgt
                    Random missRng = new Random();
                    // Een random getal van 0 t/m 2 wordt gekozen.
                    int miss = missRng.Next(0, 3);

                    // De texture wordt bepaald
                    switch (miss)
                    {
                        case 0:
                            TableB[iInUse].BackgroundImage = Properties.Resources.miss0;
                            break;
                        case 1:
                            TableB[iInUse].BackgroundImage = Properties.Resources.miss1;
                            break;
                        case 2:
                            TableB[iInUse].BackgroundImage = Properties.Resources.miss2;
                            break;
                    }

                    // De 'Tag' wordt gewijzigd naar "miss" zodat als de speler het vakje nog een keer raakt, de juiste melding naar voren komt
                    TableB[iInUse].Tag = "miss";
                    // En natuurlijk wordt "iMissesA" verhoogd met één.
                    iMissesA++;

                    // Wanneer SpecialEffects "true" is, speelt het soundeffect & de animatie. Zie regel 820 voor verdere informatie.
                    if (SpecialEffects)
                    {
                        sB.URL = "sounds/splash.mp3";
                        tmrTimer.Enabled = true;
                        StartGame = false;

                        lblPlayerA.BackColor = SystemColors.Control;
                        lblPlayerB.BackColor = Color.LightGreen;
                    }
                    else
                    {
                        EnemyShot();
                    }
                }

                // Wanneer "iHitsA" gelijk is aan "0", heeft SpelerA gewonnen
                // Edited: Wanneer "iMaxHits" gelijk is aan '0', heeft SpelerA gewonnen omdat er geen nog-te-raken vlakjes meer over zijn. Dan gaat het spel door naar de Functie "WinLoss()"
                if (iHitsA == 27)
                {
                    WinLoss();

                }
            }
            // Wanneer de game niet gestart is, wilt de speler iets doen wat nog niet kan
            else
            {
                // Wanneer de Timer aanstaat, is de 'schiet-animatie' van PC-Player nog bezig. SpelerA kan in deze tijd natuurlijk niet nog een keer schieten, anders krijgt PC-Player de kans niet een schip te raken
                if (tmrTimer.Enabled == true)
                {
                    // Dan moet SpelerA dus wachten tot hij de beurt weer heeft (dus wanneer de achtergrond van "Player A" weer groen is, "en StartGame" 'True' is)
                    MessageBox.Show("Please wait for the enemy to shoot!");
                }
                // Wanneer "btnStart" nog steeds een zwarte achtergrond heeft, is het spel geëindigd en moet de speler eerst Resetten, wil hij weer kunnen klikken
                else if (btnStart.BackColor == Color.Black)
                {
                    MessageBox.Show("You are currently in Spectator Mode! To play a new game, press RESET");
                }
                // Wanneer de bovenste 2 niet waar zijn, is het spel simpelweg nog niet gestart. Deze melding verschijnt alleen wanneer SpelerA nog boten aan het neerleggen is maar toch op TableB klikt
                else
                {
                    MessageBox.Show("The game hasn't started yet!");
                }
            }

            StatUpdate();
        }

        // Functie "EnemyShot()" doet het schieten voor de PC-Player nadat SpelerA geschoten heeft
        private void EnemyShot()
        {
            // Een int "tempHit" wordt aangemaakt om de Randoms in te gooien
            int tempHit = 0;

            // Wanneer de game is gestart, probeer te schieten voor PC-Player
            // Het is niet alsof er al eerder naar verwezen 'KAN' worden, maar mocht dit om welke reden dan ook toch gebeuren, zit er nog een 'failsafe' in
            if (StartGame)
            {
                if (HitHistory.Count == 0)
                {
                    // Wanneer de lijst HitHistory nog geen entries heeft, wordt er een nieuw nummer gegenereerd
                    GenNum();

                    // Wanneer de achtergrond van het RandomNumber zwart is, hebben we een hit. Zelfde structuur als wat uitgelegd is onder "BClick"
                    if (TableA[iRandomNumber].BackColor == Color.Black)
                    {
                        // Explosion-texture
                        TableA[iRandomNumber].BackgroundImage = Properties.Resources.txExplosion;
                        // Kleur veranderen naar iets anders dan Color.Black
                        TableA[iRandomNumber].BackColor = Color.DimGray;
                        // De tag wijzigen naar "1", dit is zodat de "GenNum()" weet welke getallen hij NIET kan kiezen als nieuwe "iRandomNumber"
                        TableA[iRandomNumber].Tag = "1";
                        // Int "iHitsB" met één verhogen
                        iHitsB++;
                        // Het Random getal toevoegen aan "HitHistory". Dit doet 2 dingen:
                        // 1. De volgende keer dat de PC-Player mag schieten, is de "HitHistory.Count" niet nul, maar één. Dan gaat hij hij dus door naar de volgende "else-if"
                        // 2. De laatst bekende 'Hit' wordt opgeslagen zodat de PC-Player in de functies hieronder rond de hit heen kan schieten (het "denken")
                        HitHistory.Add(iRandomNumber);

                        // Wanneer SpecialEffects 'True' is, speel een soundeffect.
                        if (SpecialEffects)
                        {
                            sA.URL = "sounds/allyhit.mp3";
                        }
                    }
                    // Wanneer de achtergrond van "TableA[iRandomNumber] NIET 'Color.Black' is, heeft de PC-Player het schot gemist
                    else
                    {
                        // SpecialEffects == 'True' -> Play soundeffect
                        if (SpecialEffects)
                        {
                            sA.URL = "sounds/splash.mp3";
                        }

                        // Weer wordt er een "schot gemist"-texture gegeven aan het blokje dat de PC-Player geselecteerd had om op te schieten.
                        Random missRng = new Random();
                        int miss = missRng.Next(0, 3);

                        switch (miss)
                        {
                            case 0:
                                TableA[iRandomNumber].BackgroundImage = Properties.Resources.miss0;
                                break;
                            case 1:
                                TableA[iRandomNumber].BackgroundImage = Properties.Resources.miss1;
                                break;
                            case 2:
                                TableA[iRandomNumber].BackgroundImage = Properties.Resources.miss2;
                                break;
                        }

                        // 'Tag' wordt weer "1" zodat functie "GenNum()" niet 2x hetzelfde getal kan pakken
                        TableA[iRandomNumber].Tag = "1";
                        // Omdat er gemist is, moet de computer een ander getal proberen. "HitHistory" wordt geCleared()
                        HitHistory.Clear();
                        // "iMissesB++"
                        iMissesB++;
                    }
                }
                // Wanneer er meer dan één entry in de HitHistory-list zit, betekent dat dat er al eerder een vlakje is geraakt. Daar gaan we nu op door!
                else if (HitHistory.Count == 1)
                {
                    // Een nieuwe Random() "rng" wordt gemaakt, om te bepalen welke 'kant' we op willen.
                    Random rng = new Random();

                    // "iDirection" krijgt het getal waar de Random() meer opkomt.
                    int iDirection = 0;
                    // Een Boolean wordt aangemaakt, die gebruikt wordt om te bepalen of er met succes een nog-niet-eerder-geraakt vlakje te vinden m.b.v. de "rng"-Random()
                    bool Continue = false;
                    // Int "iCounter" houdt bij hoeveel pogingen de Do-While loop hieronder heeft gedaan. Bij meer dan 'x' aantal pogingen, kapt hij eruit en maakt hij een nieuw getal aan
                    int iCounter = 0;

                    do
                    {
                        // Een nummer kiezen van één t/m 4
                        iDirection = rng.Next(1, 5);

                        switch (iDirection)
                        {
                            case 1:
                                // Rechts schieten
                                tempHit = 1;
                                break;
                            case 2:
                                // Links schieten
                                tempHit = -1;
                                break;
                            case 3:
                                // Naar beneden schieten
                                tempHit = 10;
                                break;
                            case 4:
                                // Omhoog schieten
                                tempHit = -10;
                                break;
                        }

                        // Wanneer "iDirection" kleiner is dan 3, controleren of nieuwe Int "History" + "tempHit" op de regel past. Wanneer het resulterende getal kleiner is dan 10 en 
                        //   groter of gelijk aan 0, wordt groen licht gegeven (dus Boolean "Continue" = 'true'!
                        if (iDirection < 3)
                        {
                            // Een nieuwe Int "History" krijgt de waarde van de allereerste succesvolle 'Hit' in de "HitHistory"
                            int History = HitHistory[0];

                            // Aangezien het nu om LEFT & RIGHT gaat, moet het getal verkleind worden tot een nummer onder de 10
                            while (History > 10)
                            {
                                History -= 10;
                            }

                            // Wanneer "History" + "tempHit" kleiner dan 10 is of >= 0, wordt boolean "Continue" 'true'
                            if (History + tempHit < 10 && History + tempHit >= 0)
                            {
                                Continue = true;
                            }
                            // Zo niet, "Continue" = 'false'
                            else
                            {
                                Continue = false;
                            }
                        }
                        // Als "iDirection" groter is dan 2, gaan we kijken of er omhoog of naar beneden gemanoeuvreerd kan worden. Uitdunnen hoeft hier dus niet
                        else if (iDirection > 2)
                        {
                            // Wanneer het getal kleiner is dan 100 & >= 0...
                            if (HitHistory[0] + tempHit < 100 && HitHistory[0] + tempHit >= 0)
                            {
                                // ... "Continue" == 'true'
                                Continue = true;
                            }
                            // Zo niet, "Continue" = false. Dit is om een crash te voorkomen, dat het programma niet een getal in "TableA[105]" probeert te stoppen bijvoorbeeld. Deze bestaat namelijk niet.
                            else
                            {
                                Continue = false;
                            }
                        }

                        // Een algemene check of:
                        // 1. Het getal tussen de 0 en 100 zit (weer een failsafe)
                        // 2. De 'Tag' niet "1" is, dus het vakje niet al eerder is geraakt.
                        if (HitHistory[0] + tempHit < 100 && HitHistory[0] + tempHit > 0)
                        {
                            // Als dit waar is...
                            if (TableA[HitHistory[0] + tempHit].Tag.ToString() == "1")
                            {
                                // ... is het vakje al eerder geraakt. "Continue" moet dus 'false' worden
                                Continue = false;
                            }
                        }
                        // Als het getal buiten de 0 en 100 zit, kan het sowieso niet
                        else
                        {
                            Continue = false;
                        }

                        // De teller die bijhoudt hoevaak de "do-while" loop al is gedaan, wordt met één verhoogd.
                        iCounter++;
                    }
                    // Doorgaan net zolang tot "Continue" 'true' is, of "iCounter" groter is dan 9.
                    // De "iCounter" heeft hier nog 2 functies:
                    // 1. Stel, er wordt bovenin de hoek geschoten en alle plekken eromheen zijn al geraakt. Deze 'do-while'-loop zou infinite worden
                    // 2. Als er na 'x'-aantal keer proberen geen oplossing is, pakt hij een nieuw Random getal (zie 'else' in het volgende 'if-statement'). Dit is voor een 'menselijker' gevoel
                    while (!Continue && iCounter < 10);

                    // Wanneer na de 'do-while' loop, de Boolean "Continue" 'true' is, kan er op de plek "TableA[HitHistory[0]+tempHit]" geschoten worden en gechecked of er geraakt is of gemist
                    if (Continue)
                    {
                        // Als de achtergrondkleur "Color.Black" is; Schip geraakt
                        if (TableA[HitHistory[0] + tempHit].BackColor == Color.Black)
                        {
                            TableA[HitHistory[0] + tempHit].BackgroundImage = Properties.Resources.txExplosion;
                            TableA[HitHistory[0] + tempHit].BackColor = Color.DimGray;
                            TableA[HitHistory[0] + tempHit].Tag = "1";
                            iHitsB++;
                            HitHistory.Add(HitHistory[0] + tempHit);

                            if (SpecialEffects)
                            {
                                sA.URL = "sounds/allyhit.mp3";
                            }
                        }
                        // Zo niet, weer een misser
                        else
                        {
                            Random missRng = new Random();
                            int miss = missRng.Next(0, 3);

                            switch (miss)
                            {
                                case 0:
                                    TableA[HitHistory[0] + tempHit].BackgroundImage = Properties.Resources.miss0;
                                    break;
                                case 1:
                                    TableA[HitHistory[0] + tempHit].BackgroundImage = Properties.Resources.miss1;
                                    break;
                                case 2:
                                    TableA[HitHistory[0] + tempHit].BackgroundImage = Properties.Resources.miss2;
                                    break;
                            }
                            TableA[HitHistory[0] + tempHit].Tag = "1";
                            iMissesB++;

                            if (SpecialEffects)
                            {
                                sA.URL = "sounds/splash.mp3";
                            }
                        }
                    }
                    // Wanneer boolean "Continue" na 10x proberen nog steeds niet 'true' is, list "HitHistory" clearen en een nieuw getal genereren.
                    // Daarna terug naar functie "EnemyShot()" (de functie waar we nu in zitten) om op een nieuwe random plek te schieten.
                    else
                    {
                        // Geen plek om te raken kunnen vinden, dus list "HitHistory" leegmaken en...
                        HitHistory.Clear();

                        // ... een nieuw getal genereren...
                        GenNum();
                        // ... en terug naar "EnemyShot()". Aangezien de list leeg is, gaat hij naar de "if (HitHistory.Count == 0)" dus alles begint opnieuw
                        EnemyShot();
                    }
                }

                // Wanneer er 2+ entries in de List "HitHistory" zitten, gaat PC-Player proberen om in een lijn door te schieten tot er gemist wordt
                else if (HitHistory.Count > 1)
                {
                    // Dezelfde boolean "Continue" als hierboven
                    bool Continue = false;
                    // Een Int "diff" wordt aangemaakt die dezelfde waarde krijgt als tempHit hierboven
                    // "diff" wordt berekend door de laatste en één-na-laatste entries in "HitHistory" van elkaar af te trekken
                    // Voorbeeld:
                    // Eerste 'hit' (HitHistory[0]) = 62. Tweede 'hit' (HitHistory[1]) ] 52
                    // 52-62 = -10, dus 'hit 2' is boven 'hit 1' geschoten
                    int diff = HitHistory[HitHistory.Count - 1] - HitHistory[HitHistory.Count - 2];

                    // Wanneer het verschil gelijk is aan "-1" of "1", gaat het om een verplaatsing naar rechts of links t.o.v. de één-na-laatste hit
                    if (diff == -1 || diff == 1)
                    {
                        // Wanneer de laatste entry in "HitHistory" + "diff" >= 1 && de laatste entry in "HitHistory" + "diff" < 10...
                        if (HitHistory[HitHistory.Count - 1] + diff >= 1 && HitHistory[HitHistory.Count - 1] + diff < 10)
                        {
                            // ... dan past het op de regel. Goedkeuring voor het raken van een vlakje rechts (bij 1) of links (bij -1)
                            Continue = true;
                        }
                        // Stel, de som is '-1', komt het op een regel erboven uit. Hetzelfde geldt voor wanneer de som '10' is.
                        // Dit is dus *NIET* de bedoeling, dus boolean Continue wordt 'false'
                        else
                        {
                            // Goedkeuring niet gegeven
                            Continue = false;
                        }
                    }
                    // Hetzelfde als hierboven, alleen dan met Omhoog en Omlaag. 
                    else if (diff == -10 || diff == 10)
                    {
                        if (HitHistory[HitHistory.Count - 1] + diff >= 0 && HitHistory[HitHistory.Count - 1] + diff < 100)
                        {
                            Continue = true;
                        }
                        else
                        {
                            Continue = false;
                        }
                    }

                    // Zelfde structuur als bij regel 1073, controle of het getal buiten de 0 en 100 valt of de 'Tag' "1" is.
                    // In die 2 situaties wordt "Continue" alsnog 'false'
                    if (HitHistory[HitHistory.Count - 1] + diff >= 0 && HitHistory[HitHistory.Count - 1] + diff < 100)
                    {
                        if (TableA[HitHistory[HitHistory.Count - 1] + diff].Tag.ToString() == "1")
                        {
                            Continue = false;
                        }
                    }
                    else
                    {
                        Continue = false;
                    }

                    // Wanneer "Continue" na alle checks alsnog 'true' is, proberen te schieten
                    if (Continue)
                    {
                        // Hetzelfde als eerder. Wanneer achtergrondkleur "Color.Black" is, -> 'Schip geraakt!'
                        if (TableA[HitHistory[HitHistory.Count - 1] + diff].BackColor == Color.Black)
                        {
                            TableA[HitHistory[HitHistory.Count - 1] + diff].BackgroundImage = Properties.Resources.txExplosion;
                            TableA[HitHistory[HitHistory.Count - 1] + diff].BackColor = Color.DimGray;
                            TableA[HitHistory[HitHistory.Count - 1] + diff].Tag = "1";
                            iHitsB++;
                            // Natuurlijk wordt het nummer van de net geraakte PictureBox toegevoegd aan "HitHistory"
                            HitHistory.Add(HitHistory[HitHistory.Count - 1] + diff);

                            if (SpecialEffects)
                            {
                                sA.URL = "sounds/enemyhit.mp3";
                            }
                        }
                        // Wanneer er gemist wordt
                        else
                        {
                            if (SpecialEffects)
                            {
                                sA.URL = "sounds/splash.mp3";
                            }

                            Random missRng = new Random();
                            int miss = missRng.Next(0, 3);

                            switch (miss)
                            {
                                case 0:
                                    TableA[HitHistory[HitHistory.Count - 1] + diff].BackgroundImage = Properties.Resources.miss0;
                                    break;
                                case 1:
                                    TableA[HitHistory[HitHistory.Count - 1] + diff].BackgroundImage = Properties.Resources.miss1;
                                    break;
                                case 2:
                                    TableA[HitHistory[HitHistory.Count - 1] + diff].BackgroundImage = Properties.Resources.miss2;
                                    break;
                            }

                            // Bij een misser wordt de 'Tag' weer "1"
                            TableA[HitHistory[HitHistory.Count - 1] + diff].Tag = "1";
                            iMissesB++;

                            // Wissen van de lijst, laatst geraakte entry opnieuw toevoegen zodat "HitHistory.Count == 1" andere richtingen kan proberen
                            int previous = HitHistory[HitHistory.Count - 1];

                            HitHistory.Clear();
                            HitHistory.Add(previous);
                        }
                    }
                    // Wanneer er geen 'Continue' gevonden kan worden, loopt het pad waarschijnlijk dood. 
                    else
                    {
                        // Een Int "previous" aanmaken die de laatste 'Hit' uit "HitHistory" onthoudt
                        int previous = HitHistory[HitHistory.Count - 1];

                        // De list HitHistory leegmaken
                        HitHistory.Clear();
                        // De laatst bekende 'Hit' toevoegen op plek "HitHistory[0]"
                        HitHistory.Add(previous);

                        // Aangezien er geen succesvol schot gevuurd is en daar in de huidige omstandigheden ook geen mogelijkheid tot is, gebruiken we de functie "EnemyShot()" opnieuw
                        // De reden waarom 'ie nu wel wat gaat doen, is omdat "HitHistory" nog maar één entry heeft en "EnemyShot()" dus weer richtingen kan proberen. Als er dan nog 
                        //   steeds geen plek gevonden kan worden, verwijst die weer door naar "GenNum()" om een nieuw random getal te vinden en raken.
                        EnemyShot();
                    }
                }

                // Wanneer "iHitsB" gelijk is aan 27, heeft PC-Player gewonnen.
                if (iHitsB == 27)
                {
                    // Verwijzing naar de functie "WinLoss()", die de schermen laat zien en geluiden speelt van Victory of Defeat
                    WinLoss();
                }
            }

            // Natuurlijk de statistieken verversen
            StatUpdate();
        }

        // Functie "PickMusic()" maakt een keus uit één van de 11 nummers uit de "sounds/music/"-folder en zet hem klaar in de player "sSong"
        private void PickMusic()
        {
            // Een random aanmaken om een nummer mee te kiezen
            Random whichSong = new Random();
            // Een getal kiezen
            int musicTune = whichSong.Next(1, 12);

            // De player "sSong" krijgt de URL gelijk aan het pad van het gekozen nummer. 
            // Voorbeeld: Nummer 5 wordt gekozen
            // sSong.URL = "sounds/music/music" + musictune + ".mp3";
            // sSong.URL = "sounds/music/music" + 5 + ".mp3";
            // sSong.URL = "sounds/music/music5.mp3";
            sSong.URL = "sounds/music/music" + musicTune + ".mp3";
            // Omdat na het laden van het bestand het spelen direct begint, moet de player gestopt worden zodat deze pas gaat spelen aan het begin van het spel.
            sSong.controls.stop();
        }

        // Functie "GenNumer()" genereert een random getal van 0 t/m 99 en checkt of het getal al niet eerder is gekozen door de 'Tag' te vergelijken met het getal "1".
        // Wanneer een PictureBox een 'Hit' of 'Miss' was, is de 'Tag' gewijzigd naar "1"
        private void GenNum()
        {
            // Een nieuwe Random() wordt aanegmaakt
            Random rng = new Random();
            // Twee Ints die, wanneer vermenigvuldigd, een random getal geven
            int iRen1;
            int iRen2;

            // Wanneer Startgame == false...
            if (!StartGame)
            {
                do
                {
                    // ... kiest twee getallen van 0 tot 100...
                    iRen1 = rng.Next(0, 1000);
                    iRen2 = rng.Next(0, 1000);
                    // ... vermenigvuldigt ze met elkaar...
                    iRandomNumber = iRen1 * iRen2;

                    //... dunt ze uit tot een getal onder de 100...
                    while (iRandomNumber >= 100000)
                    {
                        iRandomNumber -= 100000;
                    }

                    while (iRandomNumber >= 10000)
                    {
                        iRandomNumber -= 10000;
                    }

                    while (iRandomNumber >= 1000)
                    {
                        iRandomNumber -= 1000;
                    }

                    while (iRandomNumber >= 100)
                    {
                        iRandomNumber -= 100;
                    }
                }
                // ... net zo lang tot er een getal gevonden is dat, in "TableB[iRandomNumber]", geen zwarte achtergrond heeft
                while (TableB[iRandomNumber].BackColor == Color.Black);
            }
            // Wanneer "Startgame" = 'true'...
            else
            {
                // ... doet dezelfde stappen als hierboven...
                do
                {
                    iRen1 = rng.Next(0, 1000);
                    iRen2 = rng.Next(0, 1000);
                    iRandomNumber = iRen1 * iRen2;

                    while (iRandomNumber >= 100000)
                    {
                        iRandomNumber -= 100000;
                    }

                    while (iRandomNumber >= 10000)
                    {
                        iRandomNumber -= 10000;
                    }

                    while (iRandomNumber >= 1000)
                    {
                        iRandomNumber -= 1000;
                    }

                    while (iRandomNumber >= 100)
                    {
                        iRandomNumber -= 100;
                    }

                }
                // ... alleen dan net zo lang tot er in "TableA[iRandomNumber]" een PictureBox wordt gevonden zonder 'Tag' van "1"
                while (TableA[iRandomNumber].Tag.ToString() == "1");
            }
        }

        // De functie "GenerateBShip()" legt de schepen neer nadat SpelerA een plek voor zijn schip heeft gekozen.
        // Dit zijn dezelfde schepen. Voorbeeld: SpelerA legt een 'Battleship' neer, dus doet de PC-Player dit ook.
        // "GenerateBShip" maakt gebruik van "GenNum()" voor het random getal en "Calculate()" om te bepalen welke richtingen de schepen op kunnen
        private void GenerateBShip()
        {
            // Een Random() wordt aangemaakt voor wanneer er méér dan 1 optie is. Bijvoorbeeld: Wanneer zowel 'bUpB' als 'bLeftB' waar i 
            Random rng = new Random();
            int iB = 0;
            int iUnde = 0;

            // // Uitleg iB // //
            // Voorbeeld: Wanneer "bRight" == 'true', "iB" wordt verhoogd met één. Wanneer ook "bDown" == 'true', verhoogd met 8.
            // Totaal: 9. 'switch-case' gaat naar plek 9 waarin staat welke richting het wordt en de berekening voor het tekenen van het schip.

            // Waardes
            /// bRightB = 1
            /// bLeftB = 2
            /// bUpB = 4
            /// bDownB = 8
            // Er is voor zulke 'rare' getallen gekozen omdat op deze manier alle variaties t/m 15 kunnen en perfect aansluiten.
            // De getallenreeks is dus: 
            /// 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15

            // Waarde iB
            /// 1 RIGHT
            /// 2 LEFT
            /// 3 2+1 RIGHT LEFT
            /// 4 UP
            /// 5 4+1 UP RIGHT
            /// 6 4+2 UP LEFT
            /// 7 4+2+1 UP LEFT RIGHT
            /// 8 DOWN
            /// 9 8+1 DOWN RIGHT
            /// 10 8+2 DOWN LEFT
            /// 11 8+2+1 DOWN LEFT RIGHT
            /// 12 8+4 DOWN UP
            /// 13 8+4+1 DOWN UP RIGHT
            /// 14 8+4+2 DOWN UP LEFT
            /// 15 8+4+2+1 DOWN UP LEFT RIGHT

            // Uitleg waardes iB
            /// 1 Alleen bRightB is goed
            /// 2 Alleen bLeftB is goed
            /// 3 Zowel bRightB als bLeftB is goed
            /// 4 Alleen bUpB is goed
            /// 5 Zowel bRightB als bUpB zijn goed
            /// 6 Zowel bLeftB als bUpB zijn goed
            /// 7 Zowel bRightB, bLeftB als bUpB zijn goed
            /// 8 Alleen bDownB is goed
            /// 9 Zowel bDownB als bRightB is goed
            /// 10 Zowel bDownB als bLeftB zijn goed
            /// 11 Zowel bDownB, bLeftB en bRightB zijn goed
            /// 12 Zowel bDownB als bUpB zijn goed
            /// 13 Zowel bDownB, bUpB en bRightB zijn goed
            /// 14 Zowel bDownB, bUpB en bLeftB zijn goed
            /// 15 Alles is goed

            // Wanneer "bRightB" waar is, "iB" + 1;
            if (bRightB)
            {
                iB += 1;
            }

            // Wanneer "bLeftB" waar is, "iB" + 2;
            if (bLeftB)
            {
                iB += 2;
            }

            // Wanneer "bUpB" waar is, "iB + 4;
            if (bUpB)
            {
                iB += 4;
            }

            // Wanneer "bDownB" waar is, "iB + 8";
            if (bDownB)
            {
                iB += 8;
            }

            // 's Wereld's grootste 'switch-case'. Waarschijnlijk kan dit nog kleiner, ik heb er alleen nog geen manier voor gevonden omdat ik tijd tekort kwam.
            switch (iB)
            {
                // Wanneer "iB" '0' is gebleven en dus geen enkele Boolean 'true' was, genereer een nieuw getal, controleer met "Calculate()" en kom hier terug voor nóg een poging.
                case 0:
                    GenNum();
                    Calculate();
                    GenerateBShip();
                    break;
                case 1:
                    // Alléén bRightB = true
                    for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                    {
                        TableB[i].BackColor = Color.Black;
                    }
                    break;
                case 2:
                    // Alléén bLeftB = true
                    for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                    {
                        TableB[i].BackColor = Color.Black;
                    }
                    break;
                case 3:
                    // Zowel bLeftB als bRightB = true, random keuze
                    // Int "iUnde" wordt '1' of '2', afhankelijk van dat getal wordt het schip gelegd.
                    iUnde = rng.Next(1, 3);
                    if (iUnde == 1)
                    {  // bRightB
                        for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    {  // bLeftB
                        for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 4:
                    // Alléén bUpB = true
                    for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                    {
                        TableB[i].BackColor = Color.Black;
                    }
                    break;
                case 5:
                    // Zowel bUpB als bRightB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 3);
                    if (iUnde == 1)
                    { // bUpB
                        for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    {  // bRightB
                        for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 6:
                    // Zowel bUpB als bLeft = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 3);
                    if (iUnde == 1)
                    { // bUpB
                        for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bLeftB
                        for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 7:
                    // Zowel bRightB, bLeftB als bUpB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 4);
                    if (iUnde == 1)
                    { // bUpB
                        for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else if (iUnde == 2)
                    { // bLeftB
                        for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bRightB
                        for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 8:
                    // Alleen bDownB = true
                    for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                    {
                        TableB[i].BackColor = Color.Black;
                    }
                    break;
                case 9:
                    // Zowel bDownB als bRightB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 3);
                    if (iUnde == 1)
                    { // bDownB
                        for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bRightB
                        for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 10:
                    // Zowel bDownB als bLeftB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 3);
                    if (iUnde == 1)
                    { // bDownB
                        for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bLeftB
                        for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 11:
                    // Zowel bDownB, bLeftB en bRightB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 4);
                    if (iUnde == 1)
                    { // bDownB
                        for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else if (iUnde == 2)
                    { // bLeftB
                        for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bRightB
                        for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 12:
                    // Zowel bDownB als bUpB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 3);
                    if (iUnde == 1)
                    { // bDownB
                        for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bUpB
                        for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 13:
                    // Zowel bDownB, bUpB en bRightB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 4);
                    if (iUnde == 1)
                    { // bDownB
                        for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else if (iUnde == 2)
                    { // bUpB
                        for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bRightB
                        for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 14:
                    // Zowel bDownB, bUpB en bLeftB = true
                    // Zie "case 3"
                    iUnde = rng.Next(1, 4);
                    if (iUnde == 1)
                    { // bUpB
                        for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else if (iUnde == 2)
                    { // bDownB
                        for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bLeftB
                        for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
                case 15:
                    // Alles is goed
                    // Aangezien alles goed is, wordt er een getal gekozen van 1 t/m 4. Deze kan dus alle richtingen op.
                    iUnde = rng.Next(1, 5);
                    if (iUnde == 1)
                    { // bDownB
                        for (int i = iRandomNumber; i <= (iRandomNumber + (iShipSize * 10)); i += 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else if (iUnde == 2)
                    { // bLeftB
                        for (int i = iRandomNumber; i >= (iRandomNumber - iShipSize); i--)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else if (iUnde == 3)
                    { // bRightB
                        for (int i = iRandomNumber; i <= (iRandomNumber + iShipSize); i++)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    else
                    { // bUpB
                        for (int i = iRandomNumber; i >= (iRandomNumber - (iShipSize * 10)); i -= 10)
                        {
                            TableB[i].BackColor = Color.Black;
                        }
                    }
                    break;
            }
        }

        // Dit "KeyDown"-event maakt het plaatsen van schepen makkelijker. In plaats van steeds weer naar de linker onderhoek gaan, kun je op
        //   de nummers '1', '2' en '3' drukken op het toetsenbord voor het plaatsen van de schepen.
        private void Battleships_KeyDown(object sender, KeyEventArgs e)
        {
            // Wanneer er een toets overeen komt met wat hieronder staat, wordt de bijbehorende Button ingedrukt. Dit is zodat de "(Button)sender" nog klopt
            switch (e.KeyCode)
            {
                case Keys.D1:
                    btnAirplane.PerformClick();
                    break;
                case Keys.NumPad1:
                    btnAirplane.PerformClick();
                    break;
                case Keys.D2:
                    btnBattleship.PerformClick();
                    break;
                case Keys.NumPad2:
                    btnBattleship.PerformClick();
                    break;
                case Keys.D3:
                    btnPatrol.PerformClick();
                    break;
                case Keys.NumPad3:
                    btnPatrol.PerformClick();
                    break;
            }
        }

        // "tmrTimer" bepaalt de tijd tussen het schot van PlayerA en PC-Player. De standaard-Interval bedraagt 500, met behulp van de Trackbar op het Form kan de speler dit verhogen/verlagen
        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            // De Int "iDelay" wordt met één verhoogd
            iDelay++;

            // Na 2 'tmrTimer_Tick', dus wanneer "iDelay" == 2...
            if (iDelay == 1)
            {
                // ... wordt StartGame weer op 'true' gezet zodat SpelerA weer op het speelveld van TableB kan klikken voor het volgende schot
                StartGame = true;
                // ... "iDelay" wordt teruggezet naar "0"...
                iDelay = 0;
                // ... de functie "EnemyShot()" wordt eindelijk geactiveerd ...
                EnemyShot();
                // ... de tmrTimer zet zichzelf uit ...
                tmrTimer.Enabled = false;

                // ... en als laatste wisselen de labels "PlayerA" en "PlayerB" van achtergrondkleur om aan te duiden dat PlayerA weer aan de beurt is.
                lblPlayerB.BackColor = SystemColors.Control;
                lblPlayerA.BackColor = Color.LightGreen;
            }
        }

        // De functie "WinLoss" laat het 'Victory'- of 'Defeat'scherm zien aan het eind van een spel
        private void WinLoss()
        {
            // Wanneer de achtergrondkleur van "btnStart" *NIET* zwart is, doe dit:
            if (btnStart.BackColor != Color.Black)
            {
                // "btnStart" enabelen, tekst en kleuren wijzigen
                btnStart.Enabled = true;
                btnStart.Text = "HIDE";
                btnStart.ForeColor = Color.White;
                btnStart.BackColor = Color.Black;

                // PictureBox "pbWinLoss" en Label 'lblWinLoss" 'Visible' maken
                pbWinLoss.Visible = true;
                lblWinLoss.Visible = true;

                // Wanneer "iMaxHits == 0", heeft SpelerA gewonnen
                if (iHitsA == 27)
                {
                    // Tekst label wijzigen
                    lblWinLoss.Text = "You win!";
                    // Muziekje spelen
                    sSong.URL = "sounds/victory.mp3";
                    // De juiste foto inladen
                    pbWinLoss.BackgroundImage = Properties.Resources.icon;

                    // Ervoor zorgen dat er niet nog doorgespeeld kan worden
                    StartGame = false;
                }
                // maar wanneer "iHitsB == 27", wint de PC-Player
                else if (iHitsB == 27)
                {
                    // Tekst label wijzigen
                    lblWinLoss.Text = "You lost!";
                    // Muziekje spelen
                    sSong.URL = "sounds/defeat.mp3";
                    // De juiste foto inladen
                    pbWinLoss.BackgroundImage = Properties.Resources.icon_defeat;

                    // Ervoor zorgen dat er niet nog doorgespeeld kan worden
                    StartGame = false;
                }

                // Tekst "btnReset" wit, achtergrond zwart
                btnReset.ForeColor = Color.White;
                btnReset.BackColor = Color.Black;
            }
            // De knop die eerder bedoeld was om het spel te starten ("btnStart"), wordt nu gebruikt om het Victory-scherm te laten zien / verbergen
            // Wanneer de tekst op "btnStart" gelijk is aan "HIDE", verberg Victory-screen
            else if (btnStart.Text == "HIDE")
            {
                pbWinLoss.Visible = false;
                lblWinLoss.Visible = false;
                btnStart.Text = "SHOW";
            }
            // Wanneer de tekst gelijk is aan "SHOW", laat hem zien
            else if (btnStart.Text == "SHOW")
            {
                pbWinLoss.Visible = true;
                lblWinLoss.Visible = true;
                btnStart.Text = "HIDE";
            }

        }

        // Functie "Checkers" controleert welke CheckBox hem geactiveerd heeft en doet iets op basis daarvan
        private void Checkers(object sender, EventArgs e)
        {
            // Controleren welke Checkbox de 'sender' is
            CheckBox checker = (CheckBox)sender;
            string CheckName = checker.Name;
            // Boolean "CheckState" controleert of de CheckBox aangevinkt is of niet
            Boolean CheckState = checker.Checked;

            switch (CheckName)
            {
                // CheckBox "chkSpecial" gaat over:
                // - Soundeffects (explosions, etc)
                // - Animaties (interval)
                // De reden dat "Animatie-interval" in combinatie met "Soundeffects" uitgeschakeld wordt, is omdat als de Animaties uitstaan, de explosies/splashes door elkaar heen klinken en heel vervelend worden
                case "chkSpecial":
                    if (CheckState)
                    {
                        // Wanneer SpecialEffects 'true' is, spelen de animaties en geluiden als normaal
                        SpecialEffects = true;
                    }
                    else
                    {
                        // Zo niet, is de animatie weg en spelen er ook geen soundeffects.
                        SpecialEffects = false;
                    }
                    break;
                // "chkMusic" bepaalt of de achtergrondmuziek speelt of niet
                case "chkMusic":
                    if (CheckState)
                    {
                        // Functie "PickMusic()" kiest een nummer en bereidt het voor
                        PickMusic();
                        // Wanneer "Startgame" == 'true', speel de muziek
                        if (StartGame)
                        {
                            sSong.controls.play();
                        }
                    }
                    else
                    {
                        // Muziek wordt gestopt (en ook niet meer gestart op basis van de 'CheckState'. Dit gebeurt ergens anders in de code, wanneer er normalitair muziek gestart wordt.
                        sSong.controls.stop();
                    }
                    break;
            }
        }

        // Functie "Slider" bepaalt de stand van de Trackbars en past volume / interval daarop aan
        private void Slider(object sender, EventArgs e)
        {
            // Controleren welke Trackbar de 'sender' is
            TrackBar slider = (TrackBar)sender;
            string sliderName = slider.Name;

            // Een Int "Volume" aanmaken die de waarde van "tbVolume" overneemt. Beide gaan van 0 t/m 100 dus dit hoeft niet omgerekend te worden.
            int Volume = tBVolume.Value;

            switch (sliderName)
            {
                case "tBDelay":
                    // Afhankelijk van de waarde van "tBDelay" (van 1 t/m 5), wordt de Interval van "tmrTimer" aangepast. Hoe kleiner het getal, hoe kleiner de interval
                    switch (tBDelay.Value)
                    {
                        case 1:
                            tmrTimer.Interval = 250;
                            break;
                        case 2:
                            tmrTimer.Interval = 500;
                            break;
                        case 3:
                            tmrTimer.Interval = 750;
                            break;
                        case 4:
                            tmrTimer.Interval = 1000;
                            break;
                        case 5:
                            tmrTimer.Interval = 1200;
                            break;
                    }
                    break;
                case "tBVolume":
                    // Het volume van alle 3 de players wordt in één keer aangepast naar de waarde van "tBVolume"
                    sSong.settings.volume = Volume;
                    sA.settings.volume = Volume;
                    sB.settings.volume = Volume;
                    break;
            }
        }
    }
}
