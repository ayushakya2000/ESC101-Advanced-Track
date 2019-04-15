using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;//for interaction with UI Elements, in this case the text...
using UnityEngine.SceneManagement;


/*
 * Special Feats
 * 
 * en passant
 * promotion to any piece with menu
 * castling
 * king cant goto checkspace
 */

public class BoardManager : MonoBehaviour
{
    const float tileSize = 1.0f;

    public TextMeshProUGUI gameOver;
    public TextMeshProUGUI checkBy;

    int killNow = -1;
    int win = 0;
    public bool translateReady = false;

    static int historyEntries = 2;//static to make it available to static array
    public static int[,] historyOfMoves;//change 2 to n to get previous n moves.

    public static BoardManager BoardManagerObject { get; set; }//to make its object(its data and functions), available everywhere

    public static PieceIdentity[,] BoardPieces { get; set; }//[,] only after type, not name. Syntax.
    public static PieceIdentity[,] PromotionPieces { get; set; }
    public static PieceIdentity currentPiece = null;
    public static PieceIdentity oldSelection = null;

    private const float startSpeed = 0.15f;
    private float speed=startSpeed;//movement variables
    private Vector3 dirMove;
    private float dist = 0;
    public static float distCovered = 0;

    public bool bringPromotionOptions=false;//promotion variables
    int elevation = 5;
    public bool chosenPromotion = false;
    int choosePiece;
    private float pspeed;
    private Vector3 pdirMove;
    private float pdist = 0;
    public static float pdistCovered = 0;
    public bool turnedOnPcome = false;
    int bkp = 0;//backup of k in Enter Mode

    bool doneCastling = true;

    int xc, yc;

    int mouseOverX = -1;
    int mouseOverY = -1;

    public float rotateBy = 180;
    public bool didRotate = true;

    public static bool isWhiteTurn = true;//first turn is white

    int[,] validMoves { get; set; }

    //Lists are dynamically sized arrays
    public List<GameObject> chessmanPrefabs;//prefabs we have in this PrefabList
    private List<GameObject> activeChessman = new List<GameObject>();//pieces currently in board are stored in this list

    Quaternion orientationWhite = Quaternion.Euler(270, 270, 0);
    Quaternion orientationBlack = Quaternion.Euler(270, 90, 0);

    public void resetFunc()//for reset button
    {
        if (!translateReady && !turnedOnPcome && !bringPromotionOptions)
        {
            win = 0;
            killNow = -1;
            CamRotator.speed = 135;
            CamRotator.fixangle = 0;
            CamRotator.angle = 0;
            mouseOverX = -1;
            mouseOverY = -1;
            gameOver.text = "";
            checkBy.text = "";
            isWhiteTurn = true;
            doneCastling = true;
            translateReady = false;
            bringPromotionOptions = false;
            chosenPromotion = false;
            pdist = 0;
            pdistCovered = 0;
            turnedOnPcome = false;
            bkp = 0;
            BoardSelection.BoardSelectionObject.removeColorSelection();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (BoardPieces[i, j] != null)
                    {
                        activeChessman.Remove(BoardPieces[i, j].gameObject);
                        Destroy(BoardPieces[i, j].gameObject);
                    }
                }
            historyOfMoves = new int[historyEntries, 5];
            BoardManagerObject = this;

            spawnAll();
        }
    }

    public void backButton()
    {
        resetFunc();
        if (!translateReady && !turnedOnPcome && !bringPromotionOptions)
            SceneManager.LoadScene(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        historyOfMoves = new int[historyEntries, 5];
        BoardManagerObject = this;
        spawnAll();
    }

    // Update is called once per frame
    void Update()
    {
        createBoard();
        showMouseOver();
        if (win == 0)
        {
            if (Input.GetMouseButtonDown(0) == true)
                manageInput();
        }
        else
        {
            if (win == 1 || win == -1)
            {
                Debug.Log(win + "team wins and fixangle=" + CamRotator.fixangle + " speed=" + CamRotator.speed);//win condition... will modify later, -1 for black win, 1 for white win
                string team = (win == 1) ? "White" : "Black";
                gameOver.text = team + " Wins!!!";
            }
            else
            {
                Debug.Log("Draw by stalemate");
                gameOver.text = "Stalemate---MATCH DRAW";
            }
            CamRotator.fixangle = double.PositiveInfinity;
            if(CamRotator.speed>60)
                CamRotator.speed /= 1.001f;
        }
    }

    //FixedUpdate has fixed interval of updation usually 0.02 seconds
    private void FixedUpdate()
    {
        if (translateReady)
        {
            if (distCovered < dist)//initially distCovered and dist both are 0, so second condition
            {
                if ((distCovered / dist) <= 0.15)//smooth start, acceleration
                    speed += (0.05f*dist);
                if ((distCovered / dist) > 0.65)//smooth end, deceleration
                {
                    speed -= (0.02f * dist);
                    killNow++;
                }
                distCovered += speed * Time.deltaTime;
                currentPiece.transform.Translate((dirMove)*speed*Time.deltaTime,Space.World);//Acc to Space.Self, it moves up in z axis
            }
            else
            {
                Debug.Log("end");
                translateReady = false;
                distCovered = 0;
                dist = 0;
                killNow = -1;
                speed = startSpeed;
                currentPiece = null;
                if (validMoves[xc, yc] == 4 || validMoves[xc, yc] == 5)//promotion
                {
                    Debug.Log("Promo");
                    choosePromotion();
                }
                else if(validMoves[xc, yc] == 6 && !doneCastling)//fix here
                {
                        Debug.Log("old selection+3==" + (oldSelection.currX + 1) + " , " + (oldSelection.currY));
                        currentPiece = BoardPieces[oldSelection.currX + 1, oldSelection.currY];
                        currentPiece.hasEverMoved = true;
                        updateHistory(currentPiece.currX, currentPiece.currY, xc - 1, yc);//keeping records of previous moves
                        BoardPieces[currentPiece.currX, currentPiece.currY] = null;
                        moveTo(currentPiece.currX, currentPiece.currY, xc - 1, yc);
                        doneCastling = true;
                }
                else
                {
                    xc = yc = 0;//resetting xc and yc, we dont want to delete it if currently under promotion
                    if (!didRotate)
                    {
                        CamRotator.fixangle += rotateBy;   //rotate camera after every move
                        didRotate = true;
                    }
                    isWhiteTurn = !isWhiteTurn;
                }
                isCmOrSm();
                if(win==0)//only show if game hasnt ended
                    isCheck();
            }
            if(killNow==0)// set killNow such that this part executes only once
            {
                Debug.Log("Enters here");
                currentPiece.setXY(xc, yc);

                if (validMoves[xc, yc] == 3)//en passant mode
                {
                    activeChessman.Remove(BoardPieces[historyOfMoves[1, 3], historyOfMoves[1, 4]].gameObject);//moved from 0 to 1
                    Destroy(BoardPieces[historyOfMoves[1, 3], historyOfMoves[1, 4]].gameObject);
                }
                if (validMoves[xc, yc] == 2 || validMoves[xc, yc] == 5)//kill mode
                {
                    Debug.Log("Kill" + validMoves[xc, yc]);
                    if (BoardPieces[xc, yc].GetType() == typeof(King))
                    {
                        win = BoardPieces[xc, yc].isWhitePiece ? -1 : 1;
                    }
                    activeChessman.Remove(BoardPieces[xc, yc].gameObject);
                    Destroy(BoardPieces[xc, yc].gameObject);
                }
                BoardPieces[xc, yc] = currentPiece;
            }
        }
        if (bringPromotionOptions && win==0)//dont promote if someone has already won
        {
            if (0.01f < pdist-pdistCovered && turnedOnPcome)//initially distCovered and dist both are 0, so second condition
            {
                pdistCovered += pspeed * Time.deltaTime;
                Debug.Log("3p --- here dist=" + pdist + "  distCovered=" + pdistCovered + "  speed=" + pspeed);
                for (int i = 0; i < 4; i++)
                {
                    PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.Translate((pdirMove) * pspeed * Time.deltaTime, Space.World);//Acc to Space.Self, it moves up in z axis
                    Debug.Log("Promotion Piece " + ((isWhiteTurn) ? 0 : 1) + "," + i+"moved");
                }
            }
            else 
            {
                turnedOnPcome = false;
                if (!chosenPromotion)
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        Debug.Log("Right");
                        if (choosePiece < 5)
                        {
                            choosePiece++;
                            Debug.Log("choosePiece val=" + choosePiece);
                            for (int i = 0; i < 4; i++)
                            {
                                int k = (i >= 2) ? 1 : 2;
                                PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.position = new Vector3(PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.position.x + ((isWhiteTurn) ? -1 : 1), elevation, (isWhiteTurn) ? 7.5f : 0.5f);
                            }
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow))//else if so that only 1 key input is taken.
                    {
                        Debug.Log("Left");
                        if (choosePiece > 1)
                        {
                            choosePiece--;
                            Debug.Log("choosePiece val=" + choosePiece);
                            for (int i = 0; i < 4; i++)
                            {
                                int k = (i >= 2) ? 1 : 2;
                                PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.position = new Vector3(PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.position.x - ((isWhiteTurn) ? -1 : 1), elevation, (isWhiteTurn) ? 7.5f : 0.5f);
                            }
                        }
                    }
                    else if (Input.GetKeyDown(KeyCode.Return))
                    {
                        Debug.Log("Enter");
                        if (choosePiece != 3)
                        {
                            int k = (choosePiece > 3) ? choosePiece - 2 : choosePiece - 1;
                            bkp = k;
                            chosenPromotion = true;
                        }
                    }
                }
                else //take away tray if chosenPromotion
                {
                    pspeed = -20;
                    if (PromotionPieces[((isWhiteTurn) ? 0 : 1), bkp].transform.position.y > 0)//initially distCovered and dist both are 0, so second condition
                    {
                        pdistCovered += (pspeed) * Time.deltaTime;
                        for (int i = 0; i < 4; i++)
                        {
                            if(i!=bkp)
                                PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.Translate((pdirMove) * pspeed * Time.deltaTime, Space.World);//Acc to Space.Self, it moves up in z axis
                            else if(PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.position.y>0)//i==bkp is obvious
                                PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.Translate((new Vector3(0,-1,0)) * 6 * Time.deltaTime, Space.World);
                            Debug.Log("Promotion Piece " + ((isWhiteTurn) ? 0 : 1) + "," + i + "moved");
                        }
                    }
                    else
                    {
                        pdistCovered = 0;
                        int k = -2;
                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 2)
                                k++;
                            PromotionPieces[((isWhiteTurn) ? 0 : 1), i].transform.position = new Vector3((isWhiteTurn) ? 22.5f + k: -21.5f -k, elevation, (isWhiteTurn) ? 7.5f : 0.5f);
                            k++;
                        }

                        bkp++;//1 for queen, 2 for bishop, 3 for knight, 4 for rook and interact with user to get his choice
                        bkp += isWhiteTurn ? 0 : 6;//incrementing required values for black prefabs
                        Debug.Log("bkp===" + bkp);

                        
                        activeChessman.Remove(BoardPieces[xc, yc].gameObject);//destroying pawn in these 2 lines
                        Destroy(BoardPieces[xc, yc].gameObject);
                        spawnAt(bkp, givePos(xc, yc));
                        Debug.Log("Promo End");

                        xc = yc = 0;//resetting xc and yc, we dont want to delete it if currently under promotion
                        if (!didRotate)
                        {
                            CamRotator.fixangle += rotateBy;//rotate camera after every move
                            didRotate = true;
                        }

                        bkp = 0;
                        chosenPromotion = false;
                        pdistCovered = 0;

                        CamAdvanced.moveUp = false;

                        isWhiteTurn = !isWhiteTurn;
                        bringPromotionOptions = false;
                    }
                    
                }
                
            }
        }
    }

    void isCmOrSm()
    {
        bool noMoves = true;
        for (int i = 0; i < 8; i++)//checkMate or staleMate
        {
            for (int j = 0; j < 8; j++)
            {
                if (BoardPieces[i, j] == null)
                    continue;
                if (BoardPieces[i, j].isWhitePiece != isWhiteTurn)
                    continue;
                int[,] moves = BoardPieces[i, j].getCheckArray(BoardPieces[i, j].isAllowed(BoardPieces, true));//check moves
                if (BoardPieces[i, j].movePossible(moves))
                {
                    Debug.Log("noMoves false for ==" + i + "," + j);
                    noMoves = false;
                    break;
                }
            }
            if (!noMoves)
                break;
        }
        PieceIdentity KingPiece = BoardPieces[PieceIdentity.getKing(BoardPieces)[0, 0], PieceIdentity.getKing(BoardPieces)[1, 0]];
        Debug.Log("noMoves==" + noMoves);

        if (noMoves)
        {
            if (KingPiece.isTileUnderDanger(BoardPieces, KingPiece.currX, KingPiece.currY, isWhiteTurn))//checkmate
                win = (isWhiteTurn) ? -1 : 1;
            else//stalemate
                win = 2;
        }
    }

    void manageInput()
    {
        if (mouseOverX >= 0 && mouseOverY >= 0)
        {

            if (currentPiece != null)//move to here or unselect
                manageMovement(mouseOverX, mouseOverY);//assuming selection is already done
            else
                selectPiece();
        }
    }

    void updateHistory(int x1,int y1,int x2,int y2)
    {
        int piece=0;//1=king, 2=queen, 3=bishop, 4=knight, 5=rook, 6=pawn
        if (BoardPieces[x1, y1].GetType() == typeof(King))
            piece = 1;
        if (BoardPieces[x1, y1].GetType() == typeof(Queen))
            piece = 2;
        if (BoardPieces[x1, y1].GetType() == typeof(Bishop))
            piece = 3;
        if (BoardPieces[x1, y1].GetType() == typeof(Knight))
            piece = 4;
        if (BoardPieces[x1, y1].GetType() == typeof(Rook))
            piece = 5;
        if (BoardPieces[x1, y1].GetType() == typeof(Pawn))
            piece = 6;

        for (int i = historyEntries - 1; i >= 1; i--)
            for (int j = 0; j < 5; j++)
                historyOfMoves[i, j] = historyOfMoves[i - 1, j];

        historyOfMoves[0, 0] = piece;
        historyOfMoves[0, 1] = x1;
        historyOfMoves[0, 2] = y1;
        historyOfMoves[0, 3] = x2;
        historyOfMoves[0, 4] = y2;

    }
    
    void choosePromotion()
    {
        Debug.Log("choosePromotion");
        pdirMove = (isWhiteTurn) ? (new Vector3(-1, 0, 0)) : (new Vector3(1, 0, 0));
        pdist = (isWhiteTurn) ? 22.5f - (xc+0.5f) : 21.5f + (xc+0.5f);
        choosePiece = 3;
        pspeed = 20;
        turnedOnPcome = true;
        CamAdvanced.moveUp = true;
        bringPromotionOptions = true; 
        Debug.Log("choosePromotionEnd");
    }

    void moveTo(int x1, int y1, int x2, int y2)
    {
        dirMove = givePos(x2, y2) - givePos(x1, y1);
        dist = dirMove.magnitude;
        Debug.Log("xhere dist=" + dist); //+ "  distCovered=" + distCovered + "  speed=" + speed);
        dirMove.Normalize();
        xc = x2;
        yc = y2;
        translateReady = true;
    }

    void manageMovement(int x, int y)
    {
        if (validMoves[x,y]>0)
        {
            Debug.Log("current selection=="+(currentPiece.currX) + " , " + (currentPiece.currY));
            oldSelection = currentPiece;//helpful in castling, first king then rook, when it comes for rook to move, oldSelection has king.
            translateReady = true;
            currentPiece.hasEverMoved = true;
            updateHistory(currentPiece.currX, currentPiece.currY, x, y);//keeping records of previous moves
            BoardPieces[currentPiece.currX, currentPiece.currY] = null;
            moveTo(currentPiece.currX, currentPiece.currY, x, y);
            if (validMoves[x, y] == 6)//castling
                doneCastling = false;
            didRotate = false;
        }
        else
            currentPiece = null;
        BoardSelection.BoardSelectionObject.removeColorSelection();
    }

    void isCheck()
    {
        PieceIdentity KingPiece = BoardPieces[PieceIdentity.getKing(BoardPieces)[0, 0], PieceIdentity.getKing(BoardPieces)[1, 0]];
        if(KingPiece.isTileUnderDanger(BoardPieces, PieceIdentity.getKing(BoardPieces)[0, 0], PieceIdentity.getKing(BoardPieces)[1, 0],isWhiteTurn))
        {
            string player = (isWhiteTurn) ? "White" : "Black";
            string opponent = (!isWhiteTurn) ? "White" : "Black";
            checkBy.text = "You are under Check by " + opponent + " Team.";
        }
        else
            checkBy.text = "";
    }

    void selectPiece()
    {
        Debug.Log("outside2=" + BoardPieces[mouseOverX, mouseOverY]);

        PieceIdentity KingPiece = BoardPieces[PieceIdentity.getKing(BoardPieces)[0,0], PieceIdentity.getKing(BoardPieces)[1, 0]];
        Debug.Log("outside3=" + BoardPieces[mouseOverX, mouseOverY]);
        if (BoardPieces[mouseOverX, mouseOverY]!=null)
        {
            if (isWhiteTurn == BoardPieces[mouseOverX, mouseOverY].isWhitePiece)
            {
                int[,] moves = BoardPieces[mouseOverX, mouseOverY].isAllowed(BoardPieces, true);
                moves = BoardPieces[mouseOverX, mouseOverY].getCheckArray(moves);

                if (BoardPieces[mouseOverX, mouseOverY].movePossible(moves))
                {
                    validMoves = moves;
                    BoardSelection.BoardSelectionObject.createColorSelection(validMoves, mouseOverX, mouseOverY);
                    currentPiece = BoardPieces[mouseOverX, mouseOverY];
                }
                
            }

        }
    }

    void spawnAll()
    {
        BoardPieces = new PieceIdentity[8, 8];
        PromotionPieces = new PieceIdentity[2,4];
        
        spawnAt(0, givePos(4, 0));
        spawnAt(1, givePos(3,0));
        spawnAt(2, givePos(2,0));
        spawnAt(2, givePos(5,0));
        spawnAt(3, givePos(1,0));
        spawnAt(3, givePos(6,0));
        spawnAt(4, givePos(0,0));
        spawnAt(4, givePos(7,0));

        spawnAt(6, givePos(4, 7));
        spawnAt(7, givePos(3, 7));
        spawnAt(8, givePos(2, 7));
        spawnAt(8, givePos(5, 7));
        spawnAt(9, givePos(1, 7));
        spawnAt(9, givePos(6, 7));
        spawnAt(10, givePos(0, 7));
        spawnAt(10, givePos(7, 7));

        for (int i = 0; i < 8; i++)
        {
            spawnAt(5, givePos(i, 1));
            spawnAt(11, givePos(i, 6));
        }

        spawnAt(1, giveAirPos(20, 7),0);//To display White Promotion
        spawnAt(2, giveAirPos(21, 7),0);
        spawnAt(3, giveAirPos(23, 7),0);
        spawnAt(4, giveAirPos(24, 7),0);

        spawnAt(7, giveAirPos(-20, 0),0);//To display Black Promotion
        spawnAt(8, giveAirPos(-21, 0),0);
        spawnAt(9, giveAirPos(-23, 0),0);
        spawnAt(10, giveAirPos(-24, 0),0);

        //Stalemate pieces...
        /*
        spawnAt(0, givePos(4, 0));
        spawnAt(10, givePos(7, 0));
        //spawnAt(5, givePos(5, 3));
        spawnAt(10, givePos(7, 3));
        spawnAt(10, givePos(3, 4));
        spawnAt(10, givePos(5, 4));*/

        //Promotion test pieces

        /*
        spawnAt(5, givePos(3, 6));
        spawnAt(5, givePos(4, 6));
        spawnAt(11, givePos(3, 1));
        spawnAt(11, givePos(4, 1));*/

        //Uncomment any one of above and comment rest of it to show a demo.

    }

    //promotion spawnAt
    void spawnAt(int index, Vector3 position, int useless)//useless is just to overload and see the difference between promotion
    {
        Quaternion orientation;//learn it
        if (index <= 5)
            orientation = orientationWhite;
        else
            orientation = orientationBlack;
        GameObject gameObj = Instantiate(chessmanPrefabs[index], position, orientation) as GameObject;
        gameObj.transform.SetParent(transform); //learn it

        int x = (int)returnXandY(position).x;//x position in 2d
        int y = (int)returnXandY(position).y;

        int i = (index <= 5) ? 0 : 1;
        int diff = (index <= 5) ? 1 : 7;
        PromotionPieces[i, (index-diff)] = gameObj.GetComponent<PieceIdentity>();
        PromotionPieces[i, (index-diff)].setXY(x, y);
    }

    public Vector3 givePos(int x, int y)//gives position at middle of x,y tile
    {
        return (new Vector3((x*tileSize) + (tileSize / 2), 0, (y*tileSize) + (tileSize / 2)));
    }

    public Vector3 giveAirPos(int x, int y)//gives position at middle of x,y tile
    {
        return (new Vector3((x * tileSize) + (tileSize / 2), elevation, (y * tileSize) + (tileSize / 2)));
    }

    Vector2 returnXandY (Vector3 pos)
    {
        return (new Vector2((int)pos.x, (int)pos.z));
    }

    //Normal spawnAt
    void spawnAt(int index, Vector3 position)
    {
        Quaternion orientation;//learn it
        if(index<=5)
            orientation = orientationWhite;
        else
            orientation = orientationBlack;
        GameObject gameObj = Instantiate(chessmanPrefabs[index], position, orientation) as GameObject;
        gameObj.transform.SetParent(transform); //learn it

        int x= (int)returnXandY(position).x;//x position in 2d
        int y = (int)returnXandY(position).y;
        
        BoardPieces[x, y] = gameObj.GetComponent<PieceIdentity>();
        BoardPieces[x, y].setXY(x, y);
        activeChessman.Add(gameObj);
    }

    void showMouseOver()
    {
        if (!Camera.main)
            return;//no camera found so exit...
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f, LayerMask.GetMask("ChessPlane")))//learn it
        {
            mouseOverX = (int)hit.point.x;
            mouseOverY = (int)hit.point.z;
            
            crossTile();
        }
        else
        {
            mouseOverX = -1;
            mouseOverY = -1;
        }
    }

    void crossTile()
    {
        Debug.DrawLine(new Vector3(mouseOverX, 0, mouseOverY), new Vector3(mouseOverX + 1, 0, mouseOverY + 1));
        Debug.DrawLine(new Vector3(mouseOverX + 1, 0, mouseOverY), new Vector3(mouseOverX, 0, mouseOverY + 1));
    }

    void createBoard() //template only
    {
        int i = 0, j = 0;
        for (; i < 9; i++)
        {
            Vector3 start = new Vector3(0, 0, i);
            Vector3 end = new Vector3(8, 0, i);
            Debug.DrawLine(start, end);
            for (; j < 9; j++)
            {
                start = new Vector3(j, 0, 0);
                end = new Vector3(j, 0, 8);
                Debug.DrawLine(start, end);
            }
        }
    }
}
