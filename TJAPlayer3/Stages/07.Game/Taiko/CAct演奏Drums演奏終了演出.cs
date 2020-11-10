using System;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏Drums演奏終了演出 : CActivity
    {
        /// <summary>
        /// 課題
        /// _クリア失敗 →素材不足(確保はできる。切り出しと加工をしてないだけ。)
        /// _
        /// </summary>
        public CAct演奏Drums演奏終了演出()
        {
            base.b活性化してない = true;
        }

        public void Start()
        {
            this.ct進行メイン = new CCounter(0, 300, 22, TJAPlayer3.Timer);
            this.Back = new CCounter(0, 500, 2, TJAPlayer3.Timer);
            this.TxCounter = new CCounter(0, 600, 2, TJAPlayer3.Timer);
            // モードの決定。クリア失敗・フルコンボも事前に作っとく。
            if (TJAPlayer3.stage選曲.n確定された曲の難易度 == (int)Difficulty.Dan)
            {
                // 段位認定モード。
                if (!TJAPlayer3.stage演奏ドラム画面.actDan.GetFailedAllChallenges())
                {
                    // 段位認定モード、クリア成功
                    this.Mode[0] = EndMode.StageCleared;
                }
                else
                {
                    // 段位認定モード、クリア失敗
                    this.Mode[0] = EndMode.StageFailed;
                }
            }
            else
            {
                // 通常のモード。
                // ここでフルコンボフラグをチェックするが現時点ではない。
                // 今の段階では魂ゲージ80%以上でチェック。
                for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
                {
                    if (TJAPlayer3.stage演奏ドラム画面.nヒット数_Auto含む.Drums.Perfect == TJAPlayer3.DTX.nノーツ数[3] ||
                  TJAPlayer3.stage演奏ドラム画面.nヒット数_Auto含まない.Drums.Perfect == TJAPlayer3.DTX.nノーツ数[3])
                    {
                        this.Mode[i] = EndMode.StageDonderFullCombo;
                    }
                    else if (TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[i] >= 80 && TJAPlayer3.stage演奏ドラム画面.nヒット数_Auto含まない.Drums.Miss <= 0)
                    {
                        this.Mode[i] = EndMode.StageFullCombo;
                    }
                    else if (TJAPlayer3.stage演奏ドラム画面.actGauge.db現在のゲージ値[i] >= 80)
                    {
                        this.Mode[i] = EndMode.StageCleared;
                    }
                    else
                    {
                        this.Mode[i] = EndMode.StageFailed;
                    }
                }
            }
        }

        public override void On活性化()
        {
            this.bリザルトボイス再生済み = false;
            this.Mode = new EndMode[2];
            base.On活性化();
        }

        public override void On非活性化()
        {
            this.ct進行メイン = null;
            this.Back = null;
            base.On非活性化();
        }

        public override void OnManagedリソースの作成()
        {
            this.b再生済み = false;
            this.soundClear = TJAPlayer3.Sound管理.tサウンドを生成する(CSkin.Path(@"Sounds\Clear.ogg"), ESoundGroup.SoundEffect);
            this.Clear = EndTx("クリア成功", Color.FromArgb(240, 255, 85), Color.White);
            this.Fail = EndTx("クリア失敗", Color.FromArgb(0, 196, 249), Color.White);
            this.FullCombo = EndTx("フルコンボ", Color.FromArgb(240, 255, 85), Color.White);
            this.DonderFullCombo = EndTx("ドンダフルコンボ", Color.FromArgb(240, 255, 85), Color.White);

            this.FailCounter = new CCounter();
            base.OnManagedリソースの作成();
        }

        public override void OnManagedリソースの解放()
        {
            if (this.soundClear != null)
                this.soundClear.t解放する();
            base.OnManagedリソースの解放();
        }

        public override int On進行描画()
        {
            if (base.b初めての進行描画)
            {
                base.b初めての進行描画 = false;
            }
            if (this.ct進行メイン != null && (TJAPlayer3.stage演奏ドラム画面.eフェーズID == CStage.Eフェーズ.演奏_演奏終了演出 || TJAPlayer3.stage演奏ドラム画面.eフェーズID == CStage.Eフェーズ.演奏_STAGE_CLEAR_フェードアウト))
            {
                this.ct進行メイン.t進行();
                this.Back.t進行();
                this.TxCounter.t進行();
                this.FailCounter.t進行();

                //CDTXMania.act文字コンソール.tPrint( 0, 0, C文字コンソール.Eフォント種別.灰, this.ct進行メイン.n現在の値.ToString() );
                //仮置き
                for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
                {
                    switch (this.Mode[i])
                    {
                        case EndMode.StageDonderFullCombo:
                            //TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, TJAPlayer3.stage演奏ドラム画面.nヒット数_Auto含まない.Drums.Miss.ToString());
                            if (TJAPlayer3.Tx.Clear_Back != null)
                            {
                                TJAPlayer3.Tx.Clear_Back.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                                TJAPlayer3.Tx.Clear_Back.t2D描画(TJAPlayer3.app.Device, 1280 - (float)(Math.Sin(Back.n現在の値 * (Math.PI / 1000.0)) * 947), 192);
                            }

                            DonderFullCombo.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                            DonderFullCombo.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, (int)(1650 - (float)(Math.Sin(TxCounter.n現在の値 * (Math.PI / 950.0)) * 920)), 260);
                            break;
                        case EndMode.StageFullCombo:
                            //TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, TJAPlayer3.stage演奏ドラム画面.nヒット数_Auto含まない.Drums.Miss.ToString());
                            if (TJAPlayer3.Tx.Clear_Back != null)
                            {
                                TJAPlayer3.Tx.Clear_Back.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                                TJAPlayer3.Tx.Clear_Back.t2D描画(TJAPlayer3.app.Device, 1280 - (float)(Math.Sin(Back.n現在の値 * (Math.PI / 1000.0)) * 947), 192);
                            }

                            FullCombo.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                            FullCombo.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, (int)(1600 - (float)(Math.Sin(TxCounter.n現在の値 * (Math.PI / 900.0)) * 920)), 260);

                            break;
                        case EndMode.StageFailed:

                            if (TJAPlayer3.Tx.Clear_Back != null)
                            {
                                TJAPlayer3.Tx.fail_Back.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                                TJAPlayer3.Tx.fail_Back.t2D描画(TJAPlayer3.app.Device, 1280 - (float)(Math.Sin(Back.n現在の値 * (Math.PI / 1000.0)) * 947), 192);
                            }

                            Fail.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                            Fail.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, (int)(1600 - (float)(Math.Sin(TxCounter.n現在の値 * (Math.PI / 900.0)) * 920)), 260);
                            break;
                        case EndMode.StageCleared:
                            //TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白,TJAPlayer3.stage演奏ドラム画面.nヒット数_Auto含まない.Drums.Miss.ToString());
                            if (TJAPlayer3.Tx.Clear_Back != null)
                            {
                                TJAPlayer3.Tx.Clear_Back.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                                TJAPlayer3.Tx.Clear_Back.t2D描画(TJAPlayer3.app.Device, 1280 - (float)(Math.Sin(Back.n現在の値 * (Math.PI / 1000.0)) * 947), 192);
                            }

                            Clear.Opacity = (int)(Math.Sin(Back.n現在の値 * (Math.PI / 750.0)) * 500);
                            Clear.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, (int)(1600 - (float)(Math.Sin(TxCounter.n現在の値 * (Math.PI / 900.0)) * 920)), 260);
                            break;

                        default:
                            break;
                    }

                }



                if (this.ct進行メイン.b終了値に達した)
                {
                    if (!this.bリザルトボイス再生済み)
                    {
                        TJAPlayer3.Skin.sound成績発表.t再生する();
                        this.bリザルトボイス再生済み = true;
                    }
                    return 1;
                }
            }

            return 0;
        }

        #region[ private ]
        //-----------------
        CTexture EndTx(string str文字, Color forecolor, Color backcolor)
        {
            using (var bmp = new CPrivateFastFont(new FontFamily(TJAPlayer3.ConfigIni.FontName), 60).DrawPrivateFont(str文字, forecolor, backcolor))
            {
                return TJAPlayer3.tテクスチャの生成(bmp, false);
            }
        }

        CTexture Clear;
        CTexture Fail;
        CTexture FullCombo;
        CTexture DonderFullCombo;
        bool b再生済み;
        bool bリザルトボイス再生済み;
        CCounter ct進行メイン;
        CCounter Back;
        CCounter TxCounter;
        CCounter FailCounter;


        //CTexture[] txバチお左_成功 = new CTexture[ 5 ];
        //CTexture[] txバチお右_成功 = new CTexture[ 5 ];
        //CTexture tx文字;
        //CTexture tx文字マスク;
        CSound soundClear;
        EndMode[] Mode;
        enum EndMode
        {
            StageFailed,
            StageCleared,
            StageFullCombo,
            StageDonderFullCombo
        }
        //-----------------
        #endregion
    }
}
